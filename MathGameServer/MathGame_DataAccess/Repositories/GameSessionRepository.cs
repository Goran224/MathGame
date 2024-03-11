using MathGame_DataAccess.Interfaces;
using MathGame_Domain;
using MathGame_Domain.EntityModels;
using MathGame_Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MathGame_DataAccess.Repositories
{
    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly MathGameDbContext _dbContext;

        public GameSessionRepository(MathGameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GameSession> CreateGameSession(GameSession session)
        {
            try
            {
                await _dbContext.AddAsync(session);
                await _dbContext.SaveChangesAsync();

                return session;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<GameSession> GetLatestActiveGameSession()
        {
            try
            {
                return await _dbContext.GameSessions.Where(x => x.NumberOfPlayers < 5)
                 .Include(gs => gs.Players)
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync(s => s.Status == GameSessionStatusType.Active);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<GameSession?> GetGameSessionById(int id)
        {
            try
            {
                return await _dbContext.GameSessions.Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<GameSession> UpdateGameSession(GameSession session)
        {
            try
            {
                _dbContext.Update(session);
                await _dbContext.SaveChangesAsync();
                return session;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<List<GameSession>> GetAllActiveGameSessions()
        {
            try
            {
                return await _dbContext.GameSessions.Where(s => s.Status == GameSessionStatusType.Active).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}
