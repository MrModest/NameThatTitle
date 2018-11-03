using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NameThatTitle.Core.Models.Error
{
    public class InvalidInput
    {
        public string Value { get; }
        public string ErrorMessage { get; }

        public InvalidInput(string value, string errorMessage)
        {
            Value = value;
            ErrorMessage = errorMessage;
        }
    }
}
