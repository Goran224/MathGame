using MathGame_Domain.DtoModels;
using MathGame_Domain.EntityModels;
using MathGame_Service.Models;

namespace MathGame_Service.Interfaces
{
    public interface IPlayerService
    {
        Task<ServiceResponse<int>> RegisterAccount(PlayerDto playerDtoModel);
        Task<ServiceResponse<LoginResponseModel>> Login(LoginModel model);
        int GetLoggedPlayerIdFromHttpContext();
        Task<ServiceResponse<bool>> LogoutPlayer();
        Task<bool> IsPlayerAuthenticate();
        Task<LoggedPlayerInfo> GetPlayerByPlayerIdFromLoggedPlayerInfo(int playerId);

        Task<ServiceResponse<PlayerInfoModel>> GetLoggedUserInfo();

        Task<int> GetOnlinePlayers();
    }
}
