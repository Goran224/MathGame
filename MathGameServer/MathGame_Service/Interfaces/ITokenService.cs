using MathGame_Service.Models;

namespace MathGame_Service.Interfaces
{
    public interface ITokenService
    {
        bool GenerateJwtToken(LoginModel model, int id);
    }
}
