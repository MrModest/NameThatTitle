using System;
using System.Collections.Generic;
using System.Text;

namespace NameThatTitle.Domain.Models.Forum
{
    public class Forum : BaseEntity
    {
        public string Name { get; set; } // movie, cartoon, book, comics, anime, manga, other
        //public string Description { get; set; } //! MultiLanguage

        public virtual ICollection<Post> Posts { get; set; }
    }
}
