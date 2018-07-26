using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

namespace NameThatTitle.Domain.Utils
{
    public static class LoggingUtils
    {
        public static string ClaimsToJson(IEnumerable<Claim> claims)
        {
            if (claims == null || claims.Count() == 0)
            {
                return "[no claims]";
            }

            return JsonConvert.SerializeObject(claims.Select(c => new
            {
                type = c.Type,
                value = c.Value
            }));
        }
    }
}
