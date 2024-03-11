using AutoMapper;
using MathGame_DataAccess.Interfaces;
using MathGame_DataAccess.Repositories;
using MathGame_Domain.DtoModels;
using MathGame_Domain.EntityModels;
using MathGame_Service.Interfaces;
using MathGame_Service.Models;
using MathGame_Service.SignalR;
using MathGame_Shared.AppConstants;
using MathGame_Shared.Enums;
using MathGame_Shared.HelperMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MathGame_Service.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        private readonly ILoggedPlayersInfoRepository _loggedPlayersInfoRepository;
        private readonly IHubContext<GameHub> _hubContext;

        public PlayerService(IPlayerRepository playerRepository, ITokenService tokenService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILoggedPlayersInfoRepository loggedPlayersInfoRepository, IDistributedCache distributedCache , IHubContext<GameHub> hubContext)
        {
            _playerRepository = playerRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _distributedCache = distributedCache;
            _loggedPlayersInfoRepository = loggedPlayersInfoRepository;
            _hubContext = hubContext;
        }

        public async Task<ServiceResponse<int>> RegisterAccount(PlayerDto playerDtoModel)
        {
            var response = new ServiceResponse<int>();

            ServiceResponse<bool> validationResponse = await ValidateFieldsForRegisterAccount(playerDtoModel);
            if (!validationResponse.Success)
            {
                response.Success = validationResponse.Success;
                response.ErrorMessage = validationResponse.ErrorMessage;
                return response;
            }

            playerDtoModel.Password = Methods.GenerateSha512Hash(playerDtoModel.Password);

            var player = await _playerRepository.CreatePlayer(_mapper.Map<Player>(playerDtoModel));

            if (player == null)
            {
                response.ErrorMessage = ErrorMessages.GenericError;
                return response;
            }

            response.Data = player.Id;
            response.Success = true;
            return response;
        }

        public int GetLoggedPlayerIdFromHttpContext()
        {
            var playerIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Convert.ToInt32(playerIdClaim);
        }

        public async Task<ServiceResponse<bool>> LogoutPlayer()
        {
            var response = new ServiceResponse<bool>() { Success = false };

            // Get logged player ID from HttpContext
            var playerId = GetLoggedPlayerIdFromHttpContext();

            // Get player information from logged player info
            var validatePlayerToken = await GetPlayerByPlayerIdFromLoggedPlayerInfo(playerId);

            if (validatePlayerToken == null)
            {
                response.ErrorMessage = ErrorMessages.InvalidPlayer;
                return response;
            }

            // Check if the player is logged in
            if (validatePlayerToken.LastLogin != null && validatePlayerToken.LoginStatusId != (int)LoginStatus.LoggedOut)
            {
                // Logout player
                var loggedPlayerInfo = await _loggedPlayersInfoRepository.LogoutPlayer(playerId);
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(_configuration["Jwt:Name"]);

                response.Data = loggedPlayerInfo != null;
                response.Success = true;
                await BroadcastOnlinePlayersCount();
                return response;
            }

            response.Data = false;
            response.Success = true;
            return response;

        }


        public async Task<ServiceResponse<LoginResponseModel>> Login(LoginModel model)
        {
            var response = new ServiceResponse<LoginResponseModel>() { Success = false };

            ServiceResponse<bool> validationResponse = await ValidateLoginFields(model);
            if (!validationResponse.Success)
            {
                response.Success = validationResponse.Success;
                response.ErrorMessage = validationResponse.ErrorMessage;
                return response;
            }

            LoginResponseModel loginResponse = new();

            model.Password = Methods.GenerateSha512Hash(model.Password);

            Player player = await _playerRepository.GetPlayerByEmail(model.Email);

            if (player == null)
            {
                response.ErrorMessage = ErrorMessages.InvalidPlayer;
                return response;
            }

            // This checks into redis but for easier testing purposes i have disabled this :)
            //loginResponse.IsAccountLocked = await CheckPlayerAttempts(model.Email);
            //if (loginResponse.IsAccountLocked)
            //{
            //    response.ErrorMessage = ErrorMessages.AccountLocked;
            //    return response;
            //}

            loginResponse.IsAccountLocked = false;

            if (player.Password != model.Password) {

                response.ErrorMessage = ErrorMessages.InvalidEmailOrPassword;
                return response;
            }



            if (loginResponse.IsAccountLocked)
            {
                response.ErrorMessage = ErrorMessages.AccountLocked;
                return response;
            }

            loginResponse.isTokenGenerated = _tokenService.GenerateJwtToken(model, player.Id);

            await _loggedPlayersInfoRepository.CreateLoggedPlayerRecord(player);

            int onlinePlayersCount = await GetOnlinePlayers();


            response.Success = true;
            response.Data = loginResponse;
            await BroadcastOnlinePlayersCount();
            return response;
        }

        private async Task<bool> CheckPlayerAttempts(string playerEmail)
        {

            var numberOfAttempts = await _distributedCache.GetStringAsync(playerEmail);
            var firstFailedAttemptValue = "1";

            if (!String.IsNullOrEmpty(numberOfAttempts))
            {
                var convertednumberOfAttempts = Convert.ToInt32(numberOfAttempts);

                if (convertednumberOfAttempts > Convert.ToInt64(_configuration["Caching:FailedAttempsCounter"])) return true;

                convertednumberOfAttempts++;

                await _distributedCache.RemoveAsync(playerEmail);

                await _distributedCache.SetStringAsync(playerEmail,
                                                       convertednumberOfAttempts.ToString(),
                                                       new DistributedCacheEntryOptions
                                                       {
                                                           AbsoluteExpiration = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Caching:AccountLockingTime"]))
                                                       });

                return false;
            }
            else
            {
                await _distributedCache.SetStringAsync(playerEmail,
                                                    "1",
                                                    new DistributedCacheEntryOptions
                                                    {
                                                        AbsoluteExpiration = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Caching:AccountLockingTime"]))
                                                    });
            }

            return false;
        }


        public async Task<LoggedPlayerInfo> GetPlayerByPlayerIdFromLoggedPlayerInfo(int playerId)
        {
            return await _loggedPlayersInfoRepository.GetLoggedPlayerInfo(playerId);
        }


        #region FormValidations
        public async Task<ServiceResponse<bool>> ValidateLoginFields(LoginModel loginModel)
        {
            // Initialize the service response
            var response = new ServiceResponse<bool>() { Success = false };

            loginModel.TrimStringProperties();

            Regex emailRegex = new Regex(_configuration["RegexValidation:EmailRegex"]);
            Regex passwordRegex = new Regex(_configuration["RegexValidation:PasswordRegex"]);

            if (!emailRegex.IsMatch(loginModel.Email) || !passwordRegex.IsMatch(loginModel.Password))
            {
                response.ErrorMessage = ErrorMessages.InvalidEmailOrPassword;
                return response;
            }

            response.Data = true; // Validation successful
            response.Success = true;
            return await Task.FromResult(response);
        }

        public async Task<ServiceResponse<bool>> ValidateFieldsForRegisterAccount(PlayerDto model)
        {
            
            var response = new ServiceResponse<bool>() { Success = false };

            model.TrimStringProperties();

            Regex emailRegex = new Regex(_configuration["RegexValidation:EmailRegex"]);
            Regex passwordRegex = new Regex(_configuration["RegexValidation:PasswordRegex"]);

            if (model.Password != model.ConfirmPassword)
            {
                response.ErrorMessage = ErrorMessages.InvalidEmailOrPassword;
                return response;
            }

            Player currentPlayer =  await _playerRepository.GetPlayerByEmail(model.Email);

            if (currentPlayer != null)
            {
                response.ErrorMessage = ErrorMessages.PlayerAlreadyExist;
                return response;
            }

            if (!emailRegex.IsMatch(model.Email) || !passwordRegex.IsMatch(model.Password))
            {
                response.ErrorMessage = ErrorMessages.InvalidEmailOrPassword;
                return response;
            }

            response.Data = true; // Validation successful
            response.Success = true;
            return await Task.FromResult(response);

        }
        #endregion

        public async Task<ServiceResponse<PlayerInfoModel>> GetLoggedUserInfo()
        {
            var response = new ServiceResponse<PlayerInfoModel>() { Success = false };

            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Upn)?.Value;

            if (String.IsNullOrEmpty(userEmail))
            {
                response.ErrorMessage = ErrorMessages.InvalidPlayer;
                response.Success = false;
                return response;
            }

            var loggedPlayer = await _playerRepository.GetPlayerByEmail(userEmail);

            if (loggedPlayer == null)
            {
                response.ErrorMessage = ErrorMessages.InvalidPlayer;
                response.Success = false;
                return response;
            }

            PlayerInfoModel playerInfoModel = new()
            {
                FirstName = loggedPlayer.FirstName,
                LastName = loggedPlayer.LastName,
                Email = loggedPlayer.Email,
                PlayerScore = loggedPlayer.PlayerScore,
                GameSessionId = loggedPlayer.GameSessionId,
            };


            response.Data = playerInfoModel;
            response.Success = true;
            return response;

        }

        public async Task<bool> IsPlayerAuthenticate()
        {
            var hasCookie = _httpContextAccessor.HttpContext.Request.Cookies.ContainsKey("token");

            return await Task.FromResult(hasCookie);
        }

        public async Task<int> GetOnlinePlayers()
        {
            int onlinePlayersNumber = await _playerRepository.GetOnlinePlayers();
            return onlinePlayersNumber;

        }
        public async Task BroadcastOnlinePlayersCount()
        {
            int onlinePlayersCount = await GetOnlinePlayers();
            await _hubContext.Clients.All.SendAsync("ReceiveOnlinePlayersCount", onlinePlayersCount);
        }

    }
}
