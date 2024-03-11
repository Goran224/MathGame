using MathGame_Domain.EntityModels;
using MathGame_Service.Models;

namespace MathGame_Service.Interfaces
{
    public interface IGameExpressionService
    {
        Task<ServiceResponse<GameExpression>> CreateGameExpression(int gameSessionId);
        Task<List<GameExpression>> GetlAllGameExpressionsForGivenGameSession(int gameSessionId);
        Task<GameExpression> UpdateGameExpression(GameExpression gameExpression);
        Task<GameExpression?> GetById(int id);
        Task<ServiceResponse<int>> AnswerExpression(AnswerExpressionRequestModel answerExpressionModel);
    }
}
