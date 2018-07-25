using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NameThatTitle.Domain.Models;
using NameThatTitle.Domain.Models.Token;

namespace NameThatTitle.Domain.Interfaces.Services
{
    public interface IAccountService
    {
        Task<ResultOrErrors<OAuthToken>> SignUpAsync(string username, string email, string password);
        Task<ResultOrErrors<OAuthToken>> SignInAsync(string login, string password);
        Task<ResultOrErrors<OAuthToken>> RefreshTokenAsync(string refreshToken);
        Task<ResultOrErrors>             RevokeTokenAsync(string refreshToken);
    }
}
