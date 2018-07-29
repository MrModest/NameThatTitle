using NameThatTitle.Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace NameThatTitle.Domain.Models.Forum
{
    public class PrivateMessage : BaseEntity
    {
        public int FromId { get; set; }
        public virtual UserProfile From { get; set; }

        public int ToId { get; set; }
        public virtual UserProfile To { get; set; }

        public string Text { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } // at first equal to 'Created'
    }
}
