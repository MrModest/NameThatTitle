using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using NameThatTitle.Core.Static;
using NameThatTitle.Core.Models.Error;
using Microsoft.Extensions.Localization;

namespace NameThatTitle.Core.Extensions
{
    public static class ErrorExtensions //ToDo: replace it to separate project: 'NameThatTitle.Commons'
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

        public static void Validate(this ModelStateDictionary modelState, IStringLocalizer localizer)
        {
            if (modelState.IsValid) return;

            var modelErrors = modelState.Values
                .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                .Select(em => localizer[em].Value);

            throw new InvalidInputException(modelErrors);
        }
    }
}
