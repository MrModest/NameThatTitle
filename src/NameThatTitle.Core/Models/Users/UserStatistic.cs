using System;
using System.Collections.Generic;
using System.Text;

namespace NameThatTitle.Core.Models.Users
{
    public class UserStatistic : BaseEntity
    {
        public int Solved { get; set; } // self comments which solved post
        public int MarkedAsSolve { get; set; } // another user comments as marked as solved and weren't disputed
        public int UnmarkedAsSolve { get; set; } // another user comments as marked as solved and were disputed

        public virtual UserProfile Profile { get; set; }
    }
}
