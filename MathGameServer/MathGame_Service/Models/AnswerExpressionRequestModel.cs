using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathGame_Service.Models
{
    public class AnswerExpressionRequestModel
    {
        public int ExpressionId { get; set; }
        public double Guess { get; set; }
        public string Email { get; set; }
    }
}
