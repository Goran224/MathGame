using Hangfire;
using MathGame_Domain.EntityModels;
using MathGame_Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MathGame_Service.Services
{
    public class HangfireJobService : IHangfireJobService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public HangfireJobService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public void ScheduleGameExpressionGenerationJob()
        {
            var cronExpression = _configuration["HangfireConfig:ExpressionJobRunTime"];
            RecurringJob.AddOrUpdate("Generate Game Expressions Job", () => GenerateGameExpressions(), cronExpression);
        }

        public async Task GenerateGameExpressions()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var gameSessionService = scope.ServiceProvider.GetRequiredService<IGameSessionService>();
                var expressionService = scope.ServiceProvider.GetRequiredService<IGameExpressionService>();

                List<GameSession> activeSessions = await gameSessionService.GetAllActiveGameSessions();
                // Generate a game expression for each active session
                foreach (var session in activeSessions)
                {
                    var gameExpressionResponse = await expressionService.CreateGameExpression(session.Id);
                    session.GameExpressions.Add(gameExpressionResponse.Data);
                }
            }
        }
    }
}
