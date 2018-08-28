using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NameThatTitle.Domain.Models
{
    public class ResultOrErrors
    {
        public IEnumerable<string> Errors { get; }
        public bool Succeeded { get; }

        public static ResultOrErrors SuccessResult =>
            new ResultOrErrors(true);

        //public ResultOrErrors() { }

        protected ResultOrErrors(bool succeeded)
        {
            Succeeded = succeeded;
        }

        public ResultOrErrors(IEnumerable<string> errors)
        {
            Errors = errors;
            Succeeded = false;
        }

        public ResultOrErrors(params string[] errors) : this(errors.AsEnumerable()) { }

        public virtual void Deconstructor(out IEnumerable<string> errors)
        {
            if (Errors == null || Errors?.Count() == 0)
            {
                errors = null;
            }

            errors = Errors;
        }
    }

    public class ResultOrErrors<TResult> : ResultOrErrors
    {
        public TResult Result { get; }

        public ResultOrErrors(TResult result) : base(true)
        {
            Result = result;
        }

        public ResultOrErrors(IEnumerable<string> errors) : base(errors) { }

        public ResultOrErrors(params string[] errors) : base(errors.AsEnumerable()) { }

        public void Deconstructor(out TResult result, out IEnumerable<string> errors)
        {
            base.Deconstructor(out errors);
            result = Result;
        }
    }
}
