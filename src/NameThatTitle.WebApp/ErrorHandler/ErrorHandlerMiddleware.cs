using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NameThatTitle.Core.Models.Error;
using NameThatTitle.Core.Extensions;

namespace NameThatTitle.WebApp.ErrorHandler
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (InvalidInputException ex)
            {
                _logger.LogWarning($"Invalid input:\n{ex.Message}\n"); 
                await HandleInputValidationErrors(httpContext, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unknown error:\n{ex.Message}\n"); 
                await HandleUnknownError(httpContext, ex);
            }
        }

        private static Task HandleInputValidationErrors(HttpContext httpContext, InvalidInputException ex)
        {
            httpContext.Response.ContentType = "application/json"; //ToDo: replace it!
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return httpContext.Response.WriteAsync(ex.GetBadRequestBody());
        }

        private static Task HandleUnknownError(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json"; //ToDo: replace it!
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return httpContext.Response.WriteAsync(ex.GetInternalServerErrorBody());
        }
    }
}
