using System;
using System.Collections.Generic;
using System.Text;
using NameThatTitle.Domain.Models.Forum;

namespace NameThatTitle.Domain.Models.Users
{
    public class UserProfile : BaseEntity
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Cover { get; set; }
        public int Rating { get; set; }

        public virtual UserAccount Account { get; set; }

        public virtual UserStatistic Statistic { get; set; }

        public virtual IEnumerable<Post> Posts { get; set; }
        public virtual IEnumerable<Comment> Comments { get; set; }
    }
}
