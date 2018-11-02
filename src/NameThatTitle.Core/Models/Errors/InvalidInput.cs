using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NameThatTitle.Core.Models.Error
{
    public class InvalidInput
    {
        public string InputName { get; }
        public string InputValue { get; }
        public string Message { get; }

        public InvalidInput(string inputName, string inputValue, string message)
        {
            InputName = inputName;
            InputValue = inputValue;
            Message = message;
        }
    }
}
