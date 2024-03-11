using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathGame_Domain.EntityModels
{
    public class Player : BaseEntity
    {

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public int PlayerScore { get; set; } = 0;

        public required string Password { get; set; }

        public int? GameSessionId { get; set; }
        public GameSession GameSession { get; set; }
    }
}
