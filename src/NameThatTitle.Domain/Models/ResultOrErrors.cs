using System;
using System.Collections.Generic;
using System.Text;

namespace NameThatTitle.Domain.Models
{
    public class ResultOrErrors
    {
        public IEnumerable<string> Errors { get; set; }
        public bool Succeeded { get; set; }

        public ResultOrErrors() { }

        public ResultOrErrors(bool succeeded)
        {
            Succeeded = succeeded;
        }

        public ResultOrErrors(IEnumerable<string> errors)
        {
            Errors = errors;
            Succeeded = false;
        }

        public ResultOrErrors(string error)
        {
            Errors = new string[] { error };
            Succeeded = false;
        }
    }

    public class ResultOrErrors<TResult> : ResultOrErrors
    {
        public TResult Result { get; set; }

        public ResultOrErrors(TResult result)
        {
            Result = result;
            Succeeded = true;
        }

        public ResultOrErrors(IEnumerable<string> errors) : base(errors) { }

        public ResultOrErrors(string error) : base (error) { }
    }
}
