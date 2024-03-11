using MathGame_DataAccess.Interfaces;
using MathGame_DataAccess.Repositories;
using MathGame_Service.Interfaces;
using MathGame_Service.Services;
using MathGame_Shared.Mapper;
using Microsoft.Extensions.DependencyInjection;

namespace MathGame_Service
{
    public static class DIServiceFactory
    {
        public static void RegisterServices(IServiceCollection services)
        {            
            services.AddScoped(typeof(JWTInterceptorAuthFilter));

            services.AddScoped(typeof(IPlayerRepository), typeof(PlayerRepository));

            services.AddScoped(typeof(IPlayerService), typeof(PlayerService));

            services.AddScoped(typeof(ILoggedPlayersInfoRepository), typeof(LoggedPlayersInfoRepository));

            services.AddScoped(typeof(ITokenService), typeof(TokenService));

            services.AddScoped(typeof(IGameSessionRepository), typeof(GameSessionRepository));

            services.AddScoped(typeof(IGameSessionService), typeof(GameSessionService));

            services.AddScoped(typeof(IGameExpressionRepository), typeof(GameExpressionRepository));

            services.AddScoped(typeof(IGameExpressionService), typeof(GameExpressionService));

            // Singletons
            services.AddSingleton(AutoMapperConfig.Configure());

            services.AddSingleton<IHangfireJobService, HangfireJobService>();

        }
    }
}
