using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathGame_Service.Models
{
    public class LoginResponseModel
    {
        public string? Email { get; set; }
        public bool IsAccountLocked { get; set; }
        public bool isTokenGenerated { get; set; }

    }
}
