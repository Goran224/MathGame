using MathGame_Domain.EntityModels;

namespace MathGame_DataAccess.Interfaces
{
    public interface ILoggedPlayersInfoRepository
    {
        Task<LoggedPlayerInfo> CreateLoggedPlayerRecord(Player player);
        Task<LoggedPlayerInfo> GetLoggedPlayerInfo(int playerId, bool isFromLogout = false);
        Task<LoggedPlayerInfo> LogoutPlayer(int playerId);
    }
}
