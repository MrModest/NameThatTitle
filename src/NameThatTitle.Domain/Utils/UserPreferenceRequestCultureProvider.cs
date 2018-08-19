using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;

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

            JwtSecurityToken token;

            try
            {
                token = new JwtSecurityToken(headers[0].Replace(TokenType, "").Trim());
            }
            catch (ArgumentException ex)
            {
                return NullProviderCultureResult;
            }

            var cultureName = token.Claims.FirstOrDefault(c => c.Type == UserPreferenceParamName)?.Value;

            if (String.IsNullOrWhiteSpace(cultureName))
            {
                return NullProviderCultureResult;
            }

            return Task.FromResult(new ProviderCultureResult(cultureName));
        }
    }
}
