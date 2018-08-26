using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NameThatTitle.Domain.Interfaces.Repositories;
using NameThatTitle.Domain.Models.Token;

namespace NameThatTitle.Domain.Extensions
{
    public static class IAsyncRefreshTokenRepositoryExtensions
    {
        public async static Task<RefreshToken> AddAsync(this IAsyncRefreshTokenRepository repository, OAuthToken oAuth, int userId, string deviceName = null)
        {
            return await repository.AddAsync(new RefreshToken
            {
                Refresh = oAuth.RefreshToken,
                Access = oAuth.AccessToken,
                CreatedAt = oAuth.CreatedAt,
                UserId = userId,
                DeviceName = deviceName
            });
        }
    }
}
