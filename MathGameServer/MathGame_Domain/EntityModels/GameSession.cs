using MathGame_Shared.Enums;
using System.Linq.Expressions;
using System.Numerics;

namespace MathGame_Domain.EntityModels
{
    public class GameSession : BaseEntity
    {
        public int NumberOfPlayers { get; set; }
        public string? Winner { get; set; }
        public GameSessionStatusType Status { get; set; }   
        public ICollection<Player> Players { get; set; }
        public ICollection<GameExpression> GameExpressions { get; set; }


        public GameSession()
        {
            Players = new List<Player>();
            GameExpressions = new List<GameExpression>();
        }
    }
}
