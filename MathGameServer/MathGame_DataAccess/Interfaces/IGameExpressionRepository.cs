using MathGame_Domain.EntityModels;

namespace MathGame_DataAccess.Interfaces
{
    public interface IGameExpressionRepository
    {
        Task<GameExpression> CreateGameExpression(GameExpression gameExpression);
        Task<List<GameExpression>> GetlAllGameExpressionsForGivenGameSession(int gameSessionId);
        Task<GameExpression> UpdateGameExpression(GameExpression gameExpression);
        Task<GameExpression?> GetById(int id);
    }
}
