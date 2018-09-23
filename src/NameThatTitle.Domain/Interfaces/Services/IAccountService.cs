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
        Task<ResultOrErrors<OAuthToken, string>> SignUpAsync(string username, string email, string password);
        Task<ResultOrErrors<OAuthToken, string>> SignInAsync(string login, string password);

        Task<ResultOrErrors<OAuthToken, string>> RefreshTokenAsync(string refreshToken);
        Task<ResultOrErrors<string>>             RevokeTokenAsync(string refreshToken);
        Task<ResultOrErrors<string>>             RevokeAllTokensAsync(int userId);

        Task<ResultOrErrors<string>>             GenerateValidateTokenForResetPasswordAsync(string email);
        Task<ResultOrErrors<OAuthToken, string>> ResetPasswordAsync(int userId, string validateToken, string newPassword);

        Task<ResultOrErrors<string>>             GenerateValidateTokenForConfirmEmailAsync(int userId);
        Task<ResultOrErrors<string>>             ConfirmEmailAsync(int userId, string validateToken);

        Task<ResultOrErrors<OAuthToken, string>> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<ResultOrErrors<OAuthToken, string>> ChangePasswordAsync(int userId, string newPassword);

        Task<ResultOrErrors<string>>             GenerateValidateTokenForChangeEmailAsync(int userId, string newEmail);
        Task<ResultOrErrors<string>>             ChangeEmailAsync(int userId, string validateToken, string newEmail);

        Task<ResultOrErrors<string>>             ChangeUserNameAsync(int userId, string newUserName);
    }
}
