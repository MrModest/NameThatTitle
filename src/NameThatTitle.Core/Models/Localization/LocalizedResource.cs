using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NameThatTitle.Core.Models.Localization
{
    public class LocalizedResource : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public int CultureId { get; set; }
        public virtual Culture Culture { get; set; }
    }
}
