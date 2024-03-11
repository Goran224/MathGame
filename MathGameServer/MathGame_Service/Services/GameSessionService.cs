using MathGame_DataAccess.Interfaces;
using MathGame_DataAccess.Repositories;
using MathGame_Domain.EntityModels;
using MathGame_Service.Interfaces;
using MathGame_Service.Models;
using MathGame_Shared.Enums;
using Serilog;

namespace MathGame_Service.Services
{
    public class GameSessionService : IGameSessionService
    {

        private readonly IGameSessionRepository _gameSessionRepository;
        private readonly IPlayerRepository _playerRepository;
        public GameSessionService(IGameSessionRepository gameSessionRepository, IPlayerRepository playerRepository)
        {
            _gameSessionRepository = gameSessionRepository;
            _playerRepository = playerRepository;   

        }

        public async Task<ServiceResponse<GameSession>> CreateGameSession()
        {
            try
            {
                var response = new ServiceResponse<GameSession>() { ErrorMessage = "", Success = false };
                GameSession session = new GameSession();
                session.Status = GameSessionStatusType.Active;
                session.NumberOfPlayers = 0; 
                await _gameSessionRepository.CreateGameSession(session);
                response.Data = session;
                response.Success = true;    
                return response;
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
                return await _gameSessionRepository.GetLatestActiveGameSession();
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
                return await _gameSessionRepository.GetAllActiveGameSessions();
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
                return await _gameSessionRepository.GetGameSessionById(id);
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
                return await _gameSessionRepository.UpdateGameSession(session);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<ServiceResponse<GameSessionResponseModel>> AssignPlayerToSession(string playerEmail)
        {
            var response = new ServiceResponse<GameSessionResponseModel>();
            GameSession session;
            try
            {
                var player = await _playerRepository.GetPlayerByEmail(playerEmail);
                if (player == null)
                {
                    response.ErrorMessage = "Player not found.";
                    return response;
                }

                session = await _gameSessionRepository.GetLatestActiveGameSession();

                if (session == null)
                {
                    session = new GameSession
                    {
                        NumberOfPlayers = 0,
                        Status = GameSessionStatusType.Active
                    };
                    await _gameSessionRepository.CreateGameSession(session);
                }

                session.NumberOfPlayers++;
                session.Players.Add(player);

                await _gameSessionRepository.UpdateGameSession(session);


                response.Data = new GameSessionResponseModel
                {
                    GameSessionId = session.Id,
                    Email = playerEmail
                };

                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}
