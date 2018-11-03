using System;
using System.Collections.Generic;
using System.Linq;
using NameThatTitle.Core.Extensions;

namespace NameThatTitle.Core.Models.Error
{
    public class InvalidInputException : Exception
    {
        public IEnumerable<string> ErrorMessages { get; }

        public InvalidInputException(IEnumerable<string> invalidInputs)
            : base(invalidInputs.ToJson())
        {
            ErrorMessages = invalidInputs;
        }

        public InvalidInputException(params string[] invalidInputs)
            : this(invalidInputs.AsEnumerable()) { }
    }
}
