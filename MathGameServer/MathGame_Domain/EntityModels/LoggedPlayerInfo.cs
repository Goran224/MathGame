using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathGame_Domain.EntityModels
{
    public class LoggedPlayerInfo : BaseEntity
    {
        public int PlayerId { get; set; }
        public DateTime? LastLogin { get; set; }
        public int LoginStatusId { get; set; }

    }
}
