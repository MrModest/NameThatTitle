using NameThatTitle.Commons.Static;
using NameThatTitle.Core.Models.Error;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NameThatTitle.Core.Extensions
{
    public static class ExceptionExtensions //ToDo: replace it to separate project: 'NameThatTitle.Commons'
    {
        public static string GetBadRequestBody(this InvalidInputException ex)
        {
            return JsonConvert.SerializeObject(
                new Error((int)HttpStatusCode.BadRequest, ErrorCodes.InvalidInputError, ex.Message));
        }

        public static string GetInternalServerErrorBody(this Exception ex)
        {
            return JsonConvert.SerializeObject(
                new Error((int)HttpStatusCode.InternalServerError, ErrorCodes.UnknownError, ex.Message));
        }
    }
}
