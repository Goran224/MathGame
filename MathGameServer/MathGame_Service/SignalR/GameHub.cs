using MathGame_Domain.EntityModels;
using MathGame_Service.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MathGame_Service.SignalR
{
    public class GameHub : Hub
    {
        private readonly IPlayerService _playerService;

        public GameHub(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public async Task BroadcastNewExpression(GameExpression expression)
        {
            await Clients.All.SendAsync("ReceiveNewExpression", expression);
        }

        public async Task BroadcastUpdatedExpression(GameExpression expression)
        {
            await Clients.All.SendAsync("ReceiveUpdatedExpression", expression);
        }

        public async Task BroadcastOnlinePlayersCount(int playerCount)
        {
            await Clients.All.SendAsync("ReceivePlayerCount", playerCount);
        }
    }
}
