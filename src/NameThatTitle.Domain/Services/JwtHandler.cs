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
            var expiresIn = (long)(TimeSpan.FromMinutes(expiresMinutes).TotalMilliseconds);
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
                RefreshToken = GetRefreshToken(),
                CreatedAt = createdAt
            };

            return oAuthToken;
        }

        private string GetRefreshToken()
        {
            var token = Guid.NewGuid().ToString().Replace("-", "");

            return token;
        }
    }
}
