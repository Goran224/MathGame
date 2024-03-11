using MathGame_DataAccess.Interfaces;
using MathGame_Domain;
using MathGame_Domain.EntityModels;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MathGame_DataAccess.Repositories
{
    public class GameExpressionRepository : IGameExpressionRepository
    {
        private readonly MathGameDbContext _dbContext;

        public GameExpressionRepository(MathGameDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<GameExpression> CreateGameExpression(GameExpression gameExpression)
        {
            try
            {
                await _dbContext.GameExpressions.AddAsync(gameExpression);   
                await _dbContext.SaveChangesAsync();
                return gameExpression;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<GameExpression?> GetById(int id)
        {
            try
            {
               return await _dbContext.GameExpressions.Where(x => x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<List<GameExpression>> GetlAllGameExpressionsForGivenGameSession(int gameSessionId)
        {
            try
            {
                return await _dbContext.GameExpressions.Where(x => x.GameSessionId == gameSessionId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<GameExpression> UpdateGameExpression(GameExpression gameExpression)
        {
            try
            {
                _dbContext.Update(gameExpression);
                await _dbContext.SaveChangesAsync();
                return gameExpression;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}
