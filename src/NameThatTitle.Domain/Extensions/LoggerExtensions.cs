using Microsoft.Extensions.Logging;
using NameThatTitle.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NameThatTitle.Domain.Extensions
{
    public static class LoggerExtensions
    {
        public static void InitMethod(this ILogger logger, string methodName, params object[] args)
        {
            var argStr = args.Aggregate((c, n) => c.ToString() + ", " + n.ToString()).ToString();
            logger.LogInformation($"init {methodName}({argStr})");
        }

        public static void SuccessEndMethod(this ILogger logger, string methodName)
        {
            logger.LogInformation($"method {methodName} successfully end");
        }

        public static void SkipInvalidInput(this ILogger logger)
        {
            logger.LogWarning("skip invalid or empty input");
        }
    }
}
