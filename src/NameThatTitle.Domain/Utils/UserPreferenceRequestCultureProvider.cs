using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using NameThatTitle.Domain.Services;

namespace NameThatTitle.Domain.Utils
{
    public class UserPreferenceRequestCultureProvider : RequestCultureProvider
    {
        public string HeaderName = "Authorization";

        public string TokenType = "bearer";

        public string UserPreferenceParamName = "culture";

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var headers = httpContext.Request.GetTypedHeaders().Headers[HeaderName].AsEnumerable()?.ToList();

            if (headers == null || headers.Count == 0 || !headers[0].Contains(TokenType, StringComparison.InvariantCultureIgnoreCase))
            {
                return NullProviderCultureResult;
            }

            var accessToken = headers[0].Replace(TokenType, "").Trim();

            var claims = new JwtHandler().GetClaimsFromToken(accessToken); //? Should I use DI from httpContext for inject ITokenHandler?

            if (claims == null || claims.Count() == 0)
            {
                return NullProviderCultureResult;
            }

            var cultureName = claims.FirstOrDefault(c => c.Type == UserPreferenceParamName)?.Value;

            if (String.IsNullOrWhiteSpace(cultureName))
            {
                return NullProviderCultureResult;
            }

            return Task.FromResult(new ProviderCultureResult(cultureName));
        }
    }
}
