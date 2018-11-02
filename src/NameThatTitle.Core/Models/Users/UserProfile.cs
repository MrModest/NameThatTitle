using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NameThatTitle.Core.Models.Forum;

namespace NameThatTitle.Core.Models.Users
{
    public class UserProfile : BaseEntity
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Cover { get; set; }
        public int Rating { get; set; }

        public virtual UserAccount Account { get; set; }

        public virtual UserStatistic Statistic { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
    }
}
