using System;
using System.Collections.Generic;
using System.Linq;
using NameThatTitle.Core.Extensions;

namespace NameThatTitle.Core.Models.Error
{
    public class InvalidInputException : Exception
    {
        public IEnumerable<InvalidInput> InvalidInputs { get; }

        public InvalidInputException(IEnumerable<InvalidInput> invalidInputs)
            : base(invalidInputs.ToJson())
        {
            InvalidInputs = invalidInputs;
        }

        public InvalidInputException(params InvalidInput[] invalidInputs)
            : this(invalidInputs.AsEnumerable()) { }
    }
}
