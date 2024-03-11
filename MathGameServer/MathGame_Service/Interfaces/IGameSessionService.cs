using MathGame_Domain.EntityModels;
using MathGame_Service.Models;

namespace MathGame_Service.Interfaces
{
    public interface IGameSessionService
    {
        Task<ServiceResponse<GameSession>> CreateGameSession();
        Task<GameSession> UpdateGameSession(GameSession session);
        Task<GameSession?> GetGameSessionById(int id);
        Task<GameSession> GetLatestActiveGameSession();
        Task<ServiceResponse<GameSessionResponseModel>> AssignPlayerToSession(string playerEmail);
        Task<List<GameSession>> GetAllActiveGameSessions();

    }
}
