using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NameThatTitle.Domain.Interfaces.Services;
using NameThatTitle.Domain.Models.Token;

namespace NameThatTitle.Domain.Services
{
    public class JwtHandler : ITokenHandler
    {
        public OAuthToken Create(string secretKey, int expiresMinutes, IEnumerable<Claim> claims)
        {
            var now = DateTime.UtcNow;
            var centuryBegin = new DateTime(1970, 1, 1).ToUniversalTime();
            var expiresIn = expiresMinutes * 60 * 1000; // minutes -> milliseconds
            var createdAt = (long)((now - centuryBegin).TotalMilliseconds);

            var jwt = new JwtSecurityToken(
                    notBefore: DateTime.UtcNow,
                    claims: claims,
                    expires: now.AddMilliseconds(expiresIn),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(secretKey)),
                        SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var oAuthToken = new OAuthToken //ToDo: RefreshToken
            {
                AccessToken = encodedJwt,
                TokenType = "bearer",
                ExpiresIn = expiresIn,
                RefreshToken = GenerateRefreshToken(),
                CreatedAt = createdAt
            };

            return oAuthToken;
        }

        public IEnumerable<Claim> GetClaimsFromToken(string accessToken)
        {
            try
            {
                var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

                return token.Claims;
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException)
            {
                return null;
            }
        }

        private static string GenerateRefreshToken()
        {
            var token = Guid.NewGuid().ToString("N");

            return token;
        }
    }
}
