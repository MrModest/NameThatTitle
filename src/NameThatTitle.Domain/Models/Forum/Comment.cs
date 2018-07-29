using System;
using System.Collections.Generic;
using System.Text;
using NameThatTitle.Domain.Models.Users;

namespace NameThatTitle.Domain.Models.Forum
{
    public class Comment : BaseEntity
    {
        public int PostId { get; set; } // owned post id
        public virtual Post Post { get; set; }

        public int? ParentId { get; set; } // to whom the answer
        public virtual Comment Parent { get; set; }
        public virtual ICollection<Comment> Childs { get; set; }

        public int AuthorId { get; set; }
        public virtual UserProfile Author { get; set; }

        public string Text { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; } //ToDo: set limit

        public int Rating { get; set; } //? "-1" divide by 2 when go to karma
        public bool Offtopic { get; set; } // "I don't know the answer, but I want to say something"
        public bool CorectAnswer { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } // at first equal to 'Created'

        public bool Hide { get; set; } // for moderate (need separate table with hide post with hide reason and moderatorId)
    }
}