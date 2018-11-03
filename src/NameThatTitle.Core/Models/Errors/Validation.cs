using System;
using System.Collections.Generic;
using System.Text;
using NameThatTitle.Core.Models.Error;
using NameThatTitle.Core.Extensions;

namespace NameThatTitle.Core.Models.Errors
{
    public class Validation<T>
    {
        private readonly Func<T, bool> _validate;

        public List<string> ErrorMessages { get; } = new List<string>();

        public Validation(Func<T, bool> validator)
        {
            _validate = validator;
        }

        public Validation(Func<T, bool> validator, IEnumerable<string> errorMessages)
            : this(validator)
        {
            ErrorMessages.AddRange(errorMessages);
        }

        public Validation<T> Add(T value, string errorMessage)
        {
            if (_validate(value))
            {
                ErrorMessages.Add(errorMessage);
            }

            return this;
        }

        public void AddAndThrow(T value, string errorMessage)
        {
            if (_validate(value))
            {
                ErrorMessages.Add(errorMessage);
            }

            Throw();
        }

        public void Throw()
        {
            if (ErrorMessages.Count > 0)
            {
                throw new InvalidInputException(ErrorMessages);
            }
        }
    }

    public static class Validation
    {
        public static Validation<T> Of<T>(Func<T, bool> validator)
        {
            return new Validation<T>(validator);
        }

        public static Validation<string> NotBlankString =>
            new Validation<string>(Validators.NotBlankString);

        public static Validation<int> PositiveInt =>
            new Validation<int>(Validators.PositiveInt);

        public static Validation<N> ChangeTo<T, N>(this Validation<T> validation, Func<N, bool> validator)
        {
            return new Validation<N>(validator, validation.ErrorMessages);
        }

        public static Validation<string> ChangeToNotBlankString<T>(this Validation<T> validation)
        {
            return new Validation<string>(Validators.NotBlankString, validation.ErrorMessages);
        }

        public static Validation<int> ChangeToPositiveInt<T>(this Validation<T> validation)
        {
            return new Validation<int>(Validators.PositiveInt, validation.ErrorMessages);
        }

        public static class Validators
        {
            public static readonly Func<string, bool> NotBlankString = string.IsNullOrWhiteSpace;
            public static readonly Func<int, bool> PositiveInt = value => value > 0;
        }
    }
}
