using System;
using System.Collections.Generic;
using System.Text;
using NameThatTitle.Domain.Models.Users;

namespace NameThatTitle.Domain.Models.Forum
{
    public class Favorite
    {
        public int UserId { get; set; }
        public virtual UserProfile User { get; set; }

        public int PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
