using MathGame_DataAccess.Interfaces;
using MathGame_Domain.EntityModels;
using MathGame_Domain.Enums;
using MathGame_Service.Interfaces;
using MathGame_Service.Models;
using MathGame_Service.SignalR;
using MathGame_Shared.AppConstants;
using MathGame_Shared.HelperMethods;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Concurrent;

namespace MathGame_Service.Services
{
    public class GameExpressionService : IGameExpressionService
    {
        private readonly IGameExpressionRepository _gameExpressionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IHubContext<GameHub> _hubContext;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public GameExpressionService(IGameExpressionRepository gameExpressionRepository, IPlayerRepository playerRepository, IHubContext<GameHub> hubContext)
        {
                _gameExpressionRepository = gameExpressionRepository;
                _playerRepository = playerRepository;
                _hubContext = hubContext;
        }

        public async Task<ServiceResponse<GameExpression>> CreateGameExpression(int gameSessionId)
        {
            var response = new ServiceResponse<GameExpression>() { ErrorMessage = "", Success = false };

            try
            {
                GameExpression gameExpression = GenerateRandomExpression(gameSessionId);
                await _gameExpressionRepository.CreateGameExpression(gameExpression);
                response.Data = gameExpression;
                response.Success = true;

                // Broadcast the new expression to connected clients via SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveNewExpression", gameExpression);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                Log.Error(ex.Message);
            }

            return response;
        }

        public async Task<GameExpression?> GetById(int id)
        {
            try
            {
               return await _gameExpressionRepository.GetById(id);
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
                return await _gameExpressionRepository.GetlAllGameExpressionsForGivenGameSession(gameSessionId);
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
                return await _gameExpressionRepository.UpdateGameExpression(gameExpression);    
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        private static GameExpression GenerateRandomExpression(int sessionId)
        {
            Random random = new Random();
            int operand1 = random.Next(1, 11); // Range: 1-10
            int operand2 = random.Next(1, 11); // Range: 1-10
            string[] operators = { "+", "-", "*", "/" };
            string randomOperator = operators[random.Next(operators.Length)];

            // Calculate result
            double result = Methods.EvaluateExpression(operand1, operand2, randomOperator);

            // Create GameExpression entity
            var gameExpression = new GameExpression
            {
                MathExpression = $"{operand1} {randomOperator} {operand2}",
                Result = result,
                GameSessionId = sessionId,
                AnswerStatus = AnswerStatus.Unanswered
            };

            return gameExpression;
        }


        // Locking mechanism per expression so we do not encounter race conditions 
        private ConcurrentDictionary<int, SemaphoreSlim> _expressionLocks = new ConcurrentDictionary<int, SemaphoreSlim>();

        public async Task<ServiceResponse<int>> AnswerExpression(AnswerExpressionRequestModel answerExpressionModel)
        {
            SemaphoreSlim semaphore = _expressionLocks.GetOrAdd(answerExpressionModel.ExpressionId, new SemaphoreSlim(1, 1));

            try
            {
                 ServiceResponse<int> response = new ServiceResponse<int>();
                await semaphore.WaitAsync();

                GameExpression expression = await _gameExpressionRepository.GetById(answerExpressionModel.ExpressionId);
                Player player = await _playerRepository.GetPlayerByEmail(answerExpressionModel.Email);

                if (player == null)
                {
                    return new ServiceResponse<int>("Player not found");
                }

                response.Data = player.PlayerScore;


                if (expression == null)
                {
                    response.ErrorMessage = "Expression not found";
                    return response;
                }

                if (expression.AnswerStatus != AnswerStatus.Unanswered)
                {
                    response.ErrorMessage = ErrorMessages.ExpressionAlreadyAnswered;
                    return response;
                }

                expression.AnsweredFromPlayer = answerExpressionModel.Email;
                expression.Answer = answerExpressionModel.Guess;

                if (answerExpressionModel.Guess == expression.Result)
                {

                    player.PlayerScore++;

                    await _playerRepository.UpdatePlayer(player);

                    expression.AnswerStatus = AnswerStatus.Correct;
                }
                else
                {
                    expression.AnswerStatus = AnswerStatus.False;
                }


                GameExpression updatedExpression = await _gameExpressionRepository.UpdateGameExpression(expression);

                if (updatedExpression != null)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdatedExpression", expression);
                }

                return new ServiceResponse<int>(player.PlayerScore);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
            finally
            {
                semaphore.Release();
            }
        }

    }
}
