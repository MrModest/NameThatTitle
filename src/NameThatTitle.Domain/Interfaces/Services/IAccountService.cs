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
        Task<ResultOrErrors>             RevokeAllTokensAsync(int userId);

        Task<ResultOrErrors>             GenerateValidateTokenForResetPasswordAsync(string email);
        Task<ResultOrErrors<OAuthToken>> ResetPasswordAsync(int userId, string validateToken, string newPassword);

        Task<ResultOrErrors>             GenerateValidateTokenForConfirmEmailAsync(int userId);
        Task<ResultOrErrors>             ConfirmEmailAsync(int userId, string validateToken);

        Task<ResultOrErrors<OAuthToken>> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<ResultOrErrors<OAuthToken>> ChangePasswordAsync(int userId, string newPassword);

        Task<ResultOrErrors>             GenerateValidateTokenForChangeEmailAsync(int userId, string newEmail);
        Task<ResultOrErrors>             ChangeEmailAsync(int userId, string validateToken, string newEmail);

        Task<ResultOrErrors>             ChangeUserNameAsync(int userId, string newUserName);
    }
}
