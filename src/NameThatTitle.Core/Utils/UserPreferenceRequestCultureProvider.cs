using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using NameThatTitle.Core.Services;

namespace NameThatTitle.Core.Utils //ToDo: replace it to separate project: 'NameThatTitle.Commons'
{
    public class UserPreferenceRequestCultureProvider : RequestCultureProvider
    {
        private const string HeaderName = "Authorization";

        private const string TokenType = "bearer";

        private const string UserPreferenceParamName = "culture";

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

            if (claims == null || !claims.Any()) //ToDo: Extensions IEnumerable.IsNullOrEmpty()
            {
                return NullProviderCultureResult;
            }

            var cultureName = claims.FirstOrDefault(c => c.Type == UserPreferenceParamName)?.Value;

            if (string.IsNullOrWhiteSpace(cultureName))
            {
                return NullProviderCultureResult;
            }

            return Task.FromResult(new ProviderCultureResult(cultureName));
        }
    }
}
