using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NameThatTitle.WebApp.Infrastructure
{
    public class DbLocalizationOptions
    {
        /// <summary>
        /// If UseOnlyPropertyNames is false, this property can be used to define keys with full type names or just the name of the class
        /// </summary>
        public bool UseTypeFullNames { get; set; }

        /// <summary>
        /// This can be used to use only property names to find the keys
        /// </summary>
        public bool UseOnlyPropertyNames { get; set; }
    }
}
