using System.Threading.Tasks;
using NameThatTitle.Core.Models;
using NameThatTitle.Core.Models.Token;

namespace NameThatTitle.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Task<OAuthToken> SignUpAsync(string username, string email, string password);
        Task<OAuthToken> SignInAsync(string login, string password);

        Task<OAuthToken> RefreshTokenAsync(string refreshToken);
        Task             RevokeTokenAsync(string refreshToken);
        Task             RevokeAllTokensAsync(int userId);

        Task             GenerateValidateTokenForResetPasswordAsync(string email);
        Task<OAuthToken> ResetPasswordAsync(int userId, string validateToken, string newPassword);

        Task             GenerateValidateTokenForConfirmEmailAsync(int userId);
        Task             ConfirmEmailAsync(int userId, string validateToken);

        Task<OAuthToken> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<OAuthToken> ChangePasswordAsync(int userId, string newPassword);

        Task             GenerateValidateTokenForChangeEmailAsync(int userId, string newEmail);
        Task             ChangeEmailAsync(int userId, string validateToken, string newEmail);

        Task             ChangeUserNameAsync(int userId, string newUserName);
    }
}
