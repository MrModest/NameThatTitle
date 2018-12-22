using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NameThatTitle.Core.Extensions;
using NameThatTitle.Core.Interfaces.Repositories;
using NameThatTitle.Core.Interfaces.Services;
using NameThatTitle.Core.Models;
using NameThatTitle.Core.Models.Error;
using NameThatTitle.Core.Models.Errors;
using NameThatTitle.Core.Models.Token;
using NameThatTitle.Core.Models.Users;
using NameThatTitle.Core.Static;

namespace NameThatTitle.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService>          _logger;
        private readonly IStringLocalizer<AccountService> _localizer;

        private readonly UserManager<UserAccount>         _userManager;
        private readonly SignInManager<UserAccount>       _signInManager;

        private readonly ITokenHandler                    _tokenHandler;
        private readonly IConfiguration                   _configuration;
        private readonly IAsyncRefreshTokenRepository     _tokenRep;

        private readonly IAsyncRepository<UserProfile>    _userProfileRep;

        private readonly IEmailSender                     _emailSender;
        private readonly IEmailBodyBuilder                _emailBodyBuilder;

        public AccountService(
            ILogger<AccountService>          logger,
            IStringLocalizer<AccountService> localizer,

            UserManager<UserAccount>         userManager,
            SignInManager<UserAccount>       signInManager,

            ITokenHandler                    tokenHandler,
            IConfiguration                   configuration,
            IAsyncRefreshTokenRepository     tokenRep,

            IAsyncRepository<UserProfile>    userProfileRep,
            
            IEmailSender                     emailSender,
            IEmailBodyBuilder                emailBodyBuilder)
        {
            _logger           = logger;
            _localizer        = localizer;

            _userManager      = userManager;
            _signInManager    = signInManager;

            _tokenHandler     = tokenHandler;
            _configuration    = configuration;
            _tokenRep         = tokenRep;

            _userProfileRep   = userProfileRep;

            _emailSender      = emailSender;
            _emailBodyBuilder = emailBodyBuilder;
        }


        public async Task<OAuthToken> SignUpAsync(string username, string email, string password)
        {
            _logger.InitMethod(nameof(SignUpAsync), username, email, "pass");

            Validation.NotBlankString
                .Add(username, _localizer["Username is empty!"])
                .Add(email, _localizer["Email is empty!"])
                .AddAndThrow(password, _localizer["Password is empty!"]);

            var now = DateTime.Now;
            var userAccount = new UserAccount { UserName = username, Email = email, RegisteredAt = now, LastOnlineAt = now};

            var result = await _userManager.CreateAsync(userAccount, password); //? check "already exist"?
            if (!result.Succeeded)
            {
                _logger.LogWarning("fail creating");
                throw new InvalidInputException(result.GetErrorDescriptions());
            }

            await _userProfileRep.AddAsync(new UserProfile
            {
                Id = userAccount.Id,
                UserName = userAccount.UserName
            });

            var oAuth = await GetTokenAsync(userAccount);

            await _tokenRep.AddAsync(oAuth, userAccount.Id); //ToDo: DeviceName

            _logger.LogInformation($"successfully created new user with id: {userAccount.Id}, username: {username} and email: {email} | rTokenLength: {oAuth.RefreshToken.Length} | aToken: {oAuth.AccessToken.Length}");

            return oAuth;
        }

        public async Task<OAuthToken> SignInAsync(string login, string password)
        {
            _logger.InitMethod(nameof(SignInAsync), login, "pass");

            Validation.NotBlankString
                .Add(login, _localizer["Username/Email is empty!"])
                .AddAndThrow(password, _localizer["Password is empty!"]);

            var userAccount = await FindUserByLoginAsync(login);

            var passValidated = await _userManager.CheckPasswordAsync(userAccount, password);

            if (!passValidated)
            {
                _logger.LogWarning("password failed");

                throw new InvalidInputException(_localizer["Invalid login or password!"]);
            }

            var oAuth = await GetTokenAsync(userAccount);

            await _tokenRep.AddAsync(oAuth, userAccount.Id); //ToDo: DeviceName

            _logger.LogInformation($"user successfully signed in | rTokenLength: {oAuth.RefreshToken.Length} | aTokenLength: {oAuth.AccessToken.Length}");

            return oAuth;
        }

        public async Task<OAuthToken> RefreshTokenAsync(string refreshToken)
        {
            _logger.InitMethod(nameof(RefreshTokenAsync), $"[Length] {refreshToken?.Length}");

            Validation.NotBlankString.AddAndThrow(refreshToken, _localizer["Refresh token is Empty!"]);

            var currentToken = await _tokenRep.GetByRefreshAsync(refreshToken);

            if (currentToken == null)
            {
                _logger.LogWarning("refresh token not found");

                throw new InvalidInputException(_localizer["Invalid refresh token!"]);
            }

            var lifetime = (long)(TimeSpan.FromDays(int.Parse(_configuration["Token:RefreshLifeTimeInDays"])).TotalMilliseconds);

            var expiredDate = currentToken.CreatedAt + lifetime;

            if (DateTime.Now.Millisecond > expiredDate)
            {
                _logger.LogWarning("refresh token expired");

                throw new InvalidInputException(_localizer["Refresh token is expired!"]);
            }

            if (currentToken.UserAccount == null) //? if I replace RefreshToken from IdentityDb, it's won't work
            {
                _logger.LogError("user for valid (?!) refresh token not found");

                throw new InvalidInputException(_localizer["User for this refresh token not found!"]);
            }

            var newOAuth = await GetTokenAsync(currentToken.UserAccount);

            await _tokenRep.AddAsync(newOAuth, currentToken.UserId); //ToDo: DeviceName
            await _tokenRep.DeleteAsync(currentToken);

            _logger.LogInformation($"successfully refreshed token | rTokenLength: {newOAuth.RefreshToken.Length} | aTokenLength: {newOAuth.AccessToken.Length}");

            return newOAuth;
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            _logger.InitMethod(nameof(RevokeTokenAsync), $"[Length] {refreshToken?.Length}");

            Validation.NotBlankString.AddAndThrow(refreshToken, _localizer["Refresh token is Empty!"]);

            var currentToken = await _tokenRep.GetByRefreshAsync(refreshToken);

            if (currentToken == null)
            {
                _logger.LogWarning("refresh token not found");

                throw new InvalidInputException(_localizer["Invalid refresh token!"]);
            }

            await _tokenRep.DeleteAsync(currentToken);

            _logger.LogInformation("token successfully revoked");
        }

        public async Task RevokeAllTokensAsync(int userId)
        {
            _logger.InitMethod(nameof(RevokeAllTokensAsync), userId);

            Validation.PositiveInt.AddAndThrow(userId, _localizer["Invalid user's id!"]);

            var tokens = await _tokenRep.GetByUserIdAsync(userId);

            await _tokenRep.DeleteAsync(tokens);

            _logger.LogInformation("tokens successfully revoked");
        }

        public async Task GenerateValidateTokenForResetPasswordAsync(string email)
        {
            _logger.InitMethod(nameof(GenerateValidateTokenForResetPasswordAsync), email);

            Validation.NotBlankString.AddAndThrow(email, _localizer["Email is required!"]);

            var userAccount = await _userManager.FindByEmailAsync(email);
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                throw new InvalidInputException( _localizer["User with email '{0}' not found!", email]);
            }

            var validateToken = await _userManager.GeneratePasswordResetTokenAsync(userAccount);

            _logger.LogInformation($"validateTokenLength: {validateToken?.Length}");

            Validation.NotBlankString.AddAndThrow(validateToken, _localizer["Can't generate validate token!"]);

            await _emailSender.SendEmailAsync(new SendEmailOptions
            {
                Email = userAccount.Email,
                Subject = _localizer["Reset password"],
                Body = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.PasswordResetValidateToken, userAccount.UserName, userAccount.Id, validateToken)
            });

            _logger.LogInformation($"validate token sent to mail {userAccount.Email}");
        }

        public async Task<OAuthToken> ResetPasswordAsync(int userId, string validateToken, string newPassword)
        {
            _logger.InitMethod(nameof(ResetPasswordAsync), userId, $"[Length] {validateToken?.Length}", "newPass");

            Validation.PositiveInt
                .Add(userId, _localizer["Invalid user's id!"])
                .ChangeToNotBlankString()
                .Add(validateToken, _localizer["Validate token is empty!"])
                .AddAndThrow(newPassword, _localizer["New password is empty!"]);

            var userAccount = await FindUserByIdAsync(userId);

            var result = await _userManager.ResetPasswordAsync(userAccount, validateToken, newPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning("fail reset password");
                throw new InvalidInputException(result.GetErrorDescriptions());
            }

            _logger.LogInformation("password has changed");

            return await RevokeAllTokensAndGetNewAsync(userAccount);
        }

        public async Task GenerateValidateTokenForConfirmEmailAsync(int userId)
        {
            _logger.InitMethod(nameof(GenerateValidateTokenForConfirmEmailAsync), userId);

            Validation.PositiveInt.AddAndThrow(userId, _localizer["Invalid user's id!"]);

            var userAccount = await FindUserByIdAsync(userId);

            if (userAccount.EmailConfirmed)
            {
                _logger.LogWarning("email already confirmed");
                throw new InvalidInputException(_localizer["Email {0} is already confirmed!"]);
            }

            var validateToken = await _userManager.GenerateEmailConfirmationTokenAsync(userAccount);

            _logger.LogInformation($"validateTokenLength: {validateToken?.Length}");

            Validation.NotBlankString.AddAndThrow(validateToken, _localizer["Can't generate validate token!"]);

            await _emailSender.SendEmailAsync(new SendEmailOptions
            {
                Email   = userAccount.Email,
                Subject = _localizer["Confirm email"],
                Body    = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.ConfirmEmail, userAccount.UserName, userAccount.Id, validateToken)
            });

            _logger.LogInformation($"validate token sent to mail {userAccount.Email}");
        }

        public async Task ConfirmEmailAsync(int userId, string validateToken)
        {
            _logger.InitMethod(nameof(ConfirmEmailAsync), userId, $"[Length] {validateToken?.Length}");

            Validation.PositiveInt
                .Add(userId, _localizer["Invalid user's id!"])
                .ChangeToNotBlankString()
                .AddAndThrow(validateToken, _localizer["Validate token is empty!"]);

            var userAccount = await FindUserByIdAsync(userId);

            var result = await _userManager.ConfirmEmailAsync(userAccount, validateToken); // EmailConfirmed set true inside
            if (!result.Succeeded)
            {
                _logger.LogWarning("fail confirmation token");
                throw new InvalidInputException(result.GetErrorDescriptions());
            }

            _logger.LogInformation("email successful confirmed");

            await _emailSender.SendEmailAsync(new SendEmailOptions
            {
                Email = userAccount.Email,
                Subject = _localizer["Email confirmed"],
                Body = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.EmailConfirmed, userAccount.UserName)
            });
        }

        public async Task<OAuthToken> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            _logger.InitMethod(nameof(ChangePasswordAsync), userId, "currPass", "newPass");

            Validation.PositiveInt
                .Add(userId, _localizer["Invalid user's id!"])
                .ChangeToNotBlankString()
                .Add(currentPassword, _localizer["Old password is empty!"])
                .AddAndThrow(newPassword, _localizer["New password is empty!"]);

            var userAccount = await FindUserByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(userAccount, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning("fail changing password");
                throw new InvalidInputException(result.GetErrorDescriptions());
            }

            _logger.LogInformation("password has changed");

            return await RevokeAllTokensAndGetNewAsync(userAccount);
        }

        public async Task<OAuthToken> ChangePasswordAsync(int userId, string newPassword)
        {
            _logger.InitMethod(nameof(ChangePasswordAsync), userId, "newPass");

            Validation.PositiveInt
                .Add(userId, _localizer["Invalid user's id!"])
                .ChangeToNotBlankString()
                .AddAndThrow(newPassword, _localizer["New password is empty!"]);

            var userAccount = await FindUserByIdAsync(userId);

            userAccount.PasswordHash = _userManager.PasswordHasher.HashPassword(userAccount, newPassword);

            var result = await _userManager.UpdateAsync(userAccount);
            if (!result.Succeeded)
            {
                _logger.LogWarning("fail updating password");
                throw new InvalidInputException(result.GetErrorDescriptions());
            }

            _logger.LogInformation("password has changed");

            await _emailSender.SendEmailAsync(new SendEmailOptions
            {
                Email = userAccount.Email,
                Subject = _localizer["Password has changed"],
                Body = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.ProfileHasChanged, userAccount.UserName, "Password")
            });

            return await RevokeAllTokensAndGetNewAsync(userAccount);
        }

        public async Task GenerateValidateTokenForChangeEmailAsync(int userId, string newEmail)
        {
            _logger.InitMethod(nameof(GenerateValidateTokenForChangeEmailAsync), userId, newEmail);

            Validation.PositiveInt
                .Add(userId, _localizer["Invalid user's id!"])
                .ChangeToNotBlankString()
                .AddAndThrow(newEmail, _localizer["New email is required!"]);

            var userAccount = await FindUserByIdAsync(userId);

            var validateToken = await _userManager.GenerateChangeEmailTokenAsync(userAccount, newEmail);

            _logger.LogInformation($"validateTokenLength: {validateToken?.Length}");

            Validation.NotBlankString.AddAndThrow(validateToken, _localizer["Can't generate validate token!"]); 

            await _emailSender.SendEmailAsync(new SendEmailOptions
            {
                Email = userAccount.Email,
                Subject = _localizer["Confirm email"],
                Body = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.ConfirmEmail, userAccount.UserName, userAccount.Id, validateToken)
            });

            _logger.LogInformation($"validate token sent to mail {userAccount.Email}");
        }

        public async Task ChangeEmailAsync(int userId, string validateToken, string newEmail)
        {
            _logger.InitMethod(nameof(ChangeEmailAsync), userId, newEmail);

            Validation.PositiveInt
                .Add(userId, _localizer["Invalid user's id!"])
                .ChangeToNotBlankString()
                .Add(validateToken, _localizer["Validate token is empty!"])
                .AddAndThrow(newEmail, _localizer["New Email is empty!"]);

            var userAccount = await FindUserByIdAsync(userId);

            var result = await _userManager.ChangeEmailAsync(userAccount, newEmail, validateToken); // EmailConfirmed set true inside
            if (!result.Succeeded)
            {
                _logger.LogWarning("fail changing email");
                throw new InvalidInputException(result.GetErrorDescriptions());
            }

            _logger.LogInformation("email successful changed");

            await _emailSender.SendEmailAsync(new SendEmailOptions
            {
                Email = userAccount.Email,
                Subject = _localizer["Email has changed"],
                Body = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.ProfileHasChanged, userAccount.UserName, "Email")
            });
        }

        public async Task ChangeUserNameAsync(int userId, string newUserName)
        {
            _logger.InitMethod(nameof(ChangeUserNameAsync), userId, newUserName);

            Validation.PositiveInt
                .Add(userId, _localizer["Invalid user's id!"])
                .ChangeToNotBlankString()
                .AddAndThrow(newUserName, _localizer["New username token is empty!"]);

            var userAccount = await FindUserByIdAsync(userId);

            var result = await _userManager.SetUserNameAsync(userAccount, newUserName);
            if (!result.Succeeded)
            {
                _logger.LogWarning("fail changing username");
                throw new InvalidInputException(result.GetErrorDescriptions());
            }

            _logger.SuccessEndMethod(nameof(ChangeUserNameAsync));
        }


        private async Task<OAuthToken> GetTokenAsync(UserAccount userAccount) //ToDo: think about update claim information in token
        {
            _logger.InitMethod(nameof(GetTokenAsync), $"[UserAccount.Id] {userAccount.Id}");

            var principal = await _signInManager.CreateUserPrincipalAsync(userAccount); //ToDo: avatar, languages

            var identity = (ClaimsIdentity)principal.Identity;

            _logger.LogInformation("user claims: " + identity?.Claims.ToJson());

            if (identity == null)
            {
                return null;
            }

            var token = _tokenHandler.Create(_configuration["Token:Key"], int.Parse(_configuration["Token:LifeTimeInMinutes"]), identity.Claims);

            _logger.LogInformation($"token successfully created | rTokenLength: {token.RefreshToken.Length} | aTokenLength: {token.AccessToken.Length}");

            return token;
        }

        private async Task<OAuthToken> RevokeAllTokensAndGetNewAsync(UserAccount userAccount)
        {
            _logger.InitMethod(nameof(RevokeAllTokensAndGetNewAsync), $"[UserAccount.Id] {userAccount.Id}");

            await RevokeAllTokensAsync(userAccount.Id);

            var oAuth = await GetTokenAsync(userAccount);

            await _tokenRep.AddAsync(oAuth, userAccount.Id); //ToDo: DeviceName

            _logger.LogInformation($"new token is created | aTokenLength: {oAuth.AccessToken.Length} | rTokenLength: {oAuth.RefreshToken.Length}");

            return oAuth;
        }

        //-------------------------------- Not use zone ----------------------------------------------------------------

        private OAuthToken GetTokenByRefresh(RefreshToken refreshToken) //! if I will use this, I can't update claims data
        {
            _logger.InitMethod(nameof(GetTokenByRefresh), $"[RefreshToken.UserId] {refreshToken.UserId}");

            var claims = _tokenHandler.GetClaimsFromToken(refreshToken.Access);

            _logger.LogInformation("user claims: " + claims.ToJson());

            var token = _tokenHandler.Create(_configuration["Jwt:Key"], int.Parse(_configuration["Jwt:LifeTimeInMinutes"]), claims);

            _logger.LogInformation($"token successfully created | rTokenLength: {token.RefreshToken.Length} | aTokenLength: {token.AccessToken.Length}");

            return token;
        }

        private async Task<UserAccount> FindUserByIdAsync(int userId) //ToDo: do it better
        {
            _logger.InitMethod(nameof(FindUserByIdAsync), userId);

            var userAccount = await _userManager.FindByIdAsync(userId.ToString());
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                throw new InvalidInputException(_localizer["User with id '{0}' not found!", userId]);
            }

            _logger.LogInformation($"user found: {userAccount.UserName}");

            return userAccount;
        }

        private async Task<UserAccount> FindUserByLoginAsync(string login) //ToDo: throw InvalidInputException
        {
            _logger.InitMethod(nameof(FindUserByLoginAsync), login);

            var userAccount = await _userManager.FindByEmailOrUserNameAsync(login);
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                throw new InvalidInputException(_localizer["User with username/email {0} not found!", login]);
            }

            _logger.LogInformation($"user found: {userAccount.UserName}");

            return userAccount;
        }
    }
}
