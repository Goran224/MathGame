using MathGame_Domain.DtoModels;
using MathGame_Service.Interfaces;
using MathGame_Service.Models;
using MathGame_Service.Services;
using MathGame_Shared.AppConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;

namespace MathGame_Api.Controllers
{
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost("register-account")]
        public async Task<ActionResult<ServiceResponse<int>>> RegisterPlayer([FromBody] PlayerDto playerEntity)
        {
            try
            {
                var result = await _playerService.RegisterAccount(playerEntity);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, new ServiceResponse<int>(ErrorMessages.GenericErrorControllerMessage));
            }
        }

        [HttpPost("login-player")]
        public async Task<ActionResult<ServiceResponse<LoginResponseModel>>> LoginPlayer([FromBody] LoginModel loginModel)
        {
            try
            {
                var result = await _playerService.Login(loginModel);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, new ServiceResponse<LoginResponseModel>(ErrorMessages.GenericErrorControllerMessage));
            }
        }

        [TypeFilter(typeof(JWTInterceptorAuthFilter))]
        [HttpPost("logout")]
        public async Task<ActionResult<ServiceResponse<bool>>> Logout()
        {
            try
            {
                var result = await _playerService.LogoutPlayer();
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, new ServiceResponse<bool>(ErrorMessages.GenericErrorControllerMessage));
            }
        }

        [HttpPost("isAuthenticated")]
        public async Task<bool> IsPlayerAuthenticated()
        {
            try
            {
                var result = await _playerService.IsPlayerAuthenticate();
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        [HttpPost("getonlineplayers")]
        public async Task<int> GetOnlinePlayers()
        {
            try
            {
                var result = await _playerService.GetOnlinePlayers();
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        [Authorize]
        [HttpPost("get-logged-player-info")]
        public async Task<ServiceResponse<PlayerInfoModel>> GetLoggedUserInfo()
        {
            try
            {
                return await _playerService.GetLoggedUserInfo();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ServiceResponse<PlayerInfoModel>() { Success = false, ErrorMessage = ErrorMessages.GenericErrorControllerMessage };
            }
        }
    }
}