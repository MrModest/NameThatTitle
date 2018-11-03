using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NameThatTitle.Core.Extensions;
using NameThatTitle.Core.Interfaces.Services;

namespace NameThatTitle.WebApp.Infrastructure //ToDo: replace it to separate project: 'NameThatTitle.Commons'
{
    public class UserPreferenceRequestCultureProvider : RequestCultureProvider
    {
        private const string HeaderName = "Authorization";
        private const string TokenType = "bearer";
        private const string UserPreferenceParamName = "culture";

        private ITokenHandler _tokenHandler;
        private ILogger<UserPreferenceRequestCultureProvider> _logger;

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            _tokenHandler = GetService<ITokenHandler>(httpContext); //? haven't better way?
            _logger = GetService<ILogger<UserPreferenceRequestCultureProvider>>(httpContext); //? haven't better way?

            _logger.LogInformation("init searching language preference");

            var headers = httpContext.Request.GetTypedHeaders().Headers[HeaderName].AsEnumerable()?.ToList();

            if (headers == null || headers.Count == 0 || !headers[0].Contains(TokenType, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogInformation("no authorization: stop searching language preference");
                return NullProviderCultureResult;
            }

            var accessToken = headers[0].Replace(TokenType, "").Trim();

            var claims = _tokenHandler.GetClaimsFromToken(accessToken);

            if (claims.IsNullOrEmpty())
            {
                _logger.LogWarning("claims not found");
                return NullProviderCultureResult;
            }

            var cultureName = claims.FirstOrDefault(c => c.Type == UserPreferenceParamName)?.Value;

            if (cultureName.IsNullOrWhiteSpace())
            {
                _logger.LogInformation("claims haven't language preference");
                return NullProviderCultureResult;
            }

            _logger.LogInformation($"language preference found: {cultureName}");

            return Task.FromResult(new ProviderCultureResult(cultureName));
        }

        private static T GetService<T>(HttpContext httpContext)
        {
            return httpContext.RequestServices.GetRequiredService<T>();
        }
    }
}
