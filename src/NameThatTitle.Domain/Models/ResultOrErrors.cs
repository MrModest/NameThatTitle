using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NameThatTitle.Domain.Models
{
    public class ResultOrErrors<TError>
    {
        public IEnumerable<TError> Errors { get; }

        public bool Succeeded =>
            Errors == null || Errors.Count() == 0;

        public ResultOrErrors(IEnumerable<TError> errors)
        {
            Errors = errors;
        }

        public ResultOrErrors(params TError[] errors) 
            : this(errors.AsEnumerable()) { }

        public (object, IEnumerable<TError>) AsTuple()
        {
            return (null, Errors);
        }
    }

    public class ResultOrErrors<TResult, TError> : ResultOrErrors<TError>
    {
        public TResult Result { get; }

        public ResultOrErrors(TResult result, IEnumerable<TError> errors) : base(errors)
        {
            Result = result;
        }

        public ResultOrErrors(TResult result, params TError[] errors)
            : base(errors.AsEnumerable()) { }

        public new (TResult, IEnumerable<TError>) AsTuple()
        {
            return (Result, Errors);
        }
    }

    public static class ErrorListExtentions
    {
        public static bool IsNullOrEmpty<TError>(this IEnumerable<TError> errors)
        {
            return errors == null || errors.Count() == 0;
        }
    }
}
