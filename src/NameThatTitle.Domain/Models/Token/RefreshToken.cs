using System;
using System.Collections.Generic;
using System.Text;
using NameThatTitle.Domain.Models.Users;

namespace NameThatTitle.Domain.Models.Token
{
    public class RefreshToken
    {
        public string Refresh { get; set; }
        public string Access { get; set; }
        public long CreatedAt { get; set; }
        public int UserId { get; set; }
        public string DeviceName { get; set; }

        public virtual UserAccount UserAccount { get; set; }
    }
}
