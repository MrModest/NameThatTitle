using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using NameThatTitle.Domain.Models.Forum;
using NameThatTitle.Domain.Models.Token;

namespace NameThatTitle.Domain.Models.Users
{
    public class UserAccount : IdentityUser<int>
    {
        public DateTime RegisteredAt { get; set; }
        public DateTime LastOnlineAt { get; set; }

        public int Warning { get; set; } // in procent (0 - 100), 100 means ban

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
