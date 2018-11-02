using System;
using System.Collections.Generic;
using System.Text;
using NameThatTitle.Core.Models.Users;

namespace NameThatTitle.Core.Models.Forum
{
    public class Favorite
    {
        public int UserId { get; set; }
        public virtual UserProfile User { get; set; }

        public int PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
