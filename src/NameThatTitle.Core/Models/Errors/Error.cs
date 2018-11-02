using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NameThatTitle.Commons.Static;
using Newtonsoft.Json;

namespace NameThatTitle.Core.Models.Error
{
    public class Error
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }
        public string Message { get; }

        public Error(int code, string ErrorCode, string message)
        {
            StatusCode = code;
            Message = message;
        }

        public Error(int code, string message)
            : this(code, ErrorCodes.UnknownError, message) { }
    }
}
