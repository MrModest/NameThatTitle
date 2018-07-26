﻿using System;
using System.Collections.Generic;
using System.Text;
using NameThatTitle.Domain.Models.Users;

namespace NameThatTitle.Domain.Models.Forum
{
    public class Post : BaseEntity
    {
        public int ForumId { get; set; }
        public virtual Forum Forum { get; set; }

        public int AuthorId { get; set; }
        public virtual UserProfile Author { get; set; }

        public string Title { get; set; }
        public string Text { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; } //ToDo: set limit

        public virtual ICollection<Comment> Comments { get; set; }

        public int Rating { get; set; } // haven't "-1"; can't be negative
        public bool Solved { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } // at first equal to 'Created'

        public bool Hide { get; set; } // for moderate
    }
}