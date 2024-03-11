using MathGame_Domain.EntityModels;

namespace MathGame_DataAccess.Interfaces
{
    public interface IGameSessionRepository
    {
        Task<GameSession> CreateGameSession(GameSession session);
        Task<GameSession> UpdateGameSession(GameSession session);
        Task<GameSession?> GetGameSessionById(int id);
        Task<GameSession> GetLatestActiveGameSession();
        Task<List<GameSession>> GetAllActiveGameSessions();
    }
}
