using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NameThatTitle.Core.Interfaces.Repositories;
using NameThatTitle.Core.Models.Token;

namespace NameThatTitle.Core.Extensions
{
    public static class AsyncRefreshTokenRepositoryExtensions //ToDo: replace it to separate project: 'NameThatTitle.Commons'
    {
        public static async Task<RefreshToken> AddAsync(this IAsyncRefreshTokenRepository repository, OAuthToken oAuth, int userId, string deviceName = null)
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
