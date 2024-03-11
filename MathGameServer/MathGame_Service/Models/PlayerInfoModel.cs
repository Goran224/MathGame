using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathGame_Service.Models
{
    public class PlayerInfoModel
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public int PlayerScore { get; set; } = 0;

        public int? GameSessionId { get; set; }

    }
}
