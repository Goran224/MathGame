using MathGame_DataAccess.Interfaces;
using MathGame_Domain;
using MathGame_Domain.EntityModels;
using MathGame_Shared.AppConstants;
using MathGame_Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MathGame_DataAccess.Repositories
{
    public class LoggedPlayersInfoRepository : ILoggedPlayersInfoRepository
    {
        private readonly MathGameDbContext _dbContext;
        private readonly ILogger<LoggedPlayersInfoRepository> _logger;

        public LoggedPlayersInfoRepository(MathGameDbContext dbContext, ILogger<LoggedPlayersInfoRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<LoggedPlayerInfo> CreateLoggedPlayerRecord(Player player)
        {
            try
            {
                var playerisLogged = await _dbContext.LoggedPlayersInfo.Where(x => x.PlayerId == player.Id && x.LoginStatusId == (int)LoginStatus.LoggedIn)
                                                                   .OrderByDescending(x => x.LastLogin)
                                                                   .FirstOrDefaultAsync();
                if (playerisLogged != null)
                {
                    playerisLogged.LastLogin = DateTime.Now;
                    await _dbContext.SaveChangesAsync();
                    return playerisLogged;
                }

                var loggedPlayer = new LoggedPlayerInfo()
                {
                    PlayerId = player.Id,
                    LastLogin = DateTime.Now,
                    LoginStatusId = (int)LoginStatus.LoggedIn
                };

                await _dbContext.AddAsync(loggedPlayer);
                await _dbContext.SaveChangesAsync();

                return loggedPlayer;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

        public async Task<LoggedPlayerInfo> LogoutPlayer(int playerId)
        {
            try
            {
                var loggedPlayerInfo = await GetLoggedPlayerInfo(playerId, true);

                if (loggedPlayerInfo is null) throw new Exception(ErrorMessages.GenericError);

                loggedPlayerInfo.LoginStatusId = (int)LoginStatus.LoggedOut;
                await _dbContext.SaveChangesAsync();

                return loggedPlayerInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

        public async Task<LoggedPlayerInfo> GetLoggedPlayerInfo(int playerId, bool isFromLogout = false)
        {
            try
            {
                return await _dbContext.LoggedPlayersInfo
                                        .Where(x => x.PlayerId == playerId && (!isFromLogout || x.LoginStatusId == (int)LoginStatus.LoggedIn))
                                        .OrderByDescending(x => x.LastLogin)
                                        .FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
    }
}
