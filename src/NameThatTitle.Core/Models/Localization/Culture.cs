using System;
using System.Collections.Generic;
using System.Text;

namespace NameThatTitle.Core.Models.Localization
{
    public class Culture : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<LocalizedResource> Resources { get; set; }
    }
}
