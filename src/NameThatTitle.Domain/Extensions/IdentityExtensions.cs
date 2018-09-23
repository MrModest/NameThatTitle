using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace NameThatTitle.Domain.Extensions
{
    public static class IdentityExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            if (user == null || !user.Identity.IsAuthenticated) { return -1; }

            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdStr == null) { return -1; }

            return int.Parse(userIdStr);
        }

        public static IEnumerable<string> GetErrorDescriptions(this IdentityResult ir)
        {
            return ir.Errors.Select(e => e.Description);
        }

        public static string ToJson(this IEnumerable<Claim> claims)
        {
            if (claims == null || claims.Count() == 0)
            {
                return JsonConvert.SerializeObject(Array.Empty<Claim>());
            }

            return JsonConvert.SerializeObject(claims.Select(c => new
            {
                type = c.Type,
                value = c.Value
            }));
        }

        public static (object, IEnumerable<string>) AsTuple(this IdentityResult ir)
        {
            return (null, ir.GetErrorDescriptions());
        }
    }
}
