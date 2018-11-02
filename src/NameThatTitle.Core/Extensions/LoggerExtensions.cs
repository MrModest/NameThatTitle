using Microsoft.Extensions.Logging;
using NameThatTitle.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NameThatTitle.Core.Extensions
{
    public static class LoggerExtensions //ToDo: replace it to separate project: 'NameThatTitle.Commons'
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
