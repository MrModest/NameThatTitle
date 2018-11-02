using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using NameThatTitle.Core.Models.Forum;
using NameThatTitle.Core.Models.Token;

namespace NameThatTitle.Core.Models.Users
{
    public class UserAccount : IdentityUser<int>
    {
        public DateTime RegisteredAt { get; set; }
        public DateTime LastOnlineAt { get; set; }

        //? account or profile?
        public int Warning { get; set; } // in percent (0 - 100), 100 means ban

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
