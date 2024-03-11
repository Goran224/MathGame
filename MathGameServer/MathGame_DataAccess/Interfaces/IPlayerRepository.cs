using MathGame_Domain.EntityModels;

namespace MathGame_DataAccess.Interfaces
{
    public interface IPlayerRepository
    {
        Task<Player> CreatePlayer(Player player);
        Task<Player?> GetPlayer(string email, string password);
        Task<Player?> GetPlayerByEmail(string email);
        Task<Player?> UpdatePlayer(Player player);
        Task<int> GetOnlinePlayers();
    }
}
