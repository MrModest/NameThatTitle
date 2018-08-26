using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NameThatTitle.Domain.Extensions;
using NameThatTitle.Domain.Interfaces.Repositories;
using NameThatTitle.Domain.Interfaces.Services;
using NameThatTitle.Domain.Models;
using NameThatTitle.Domain.Models.Token;
using NameThatTitle.Domain.Models.Users;
using NameThatTitle.Domain.Static;

namespace NameThatTitle.Domain.Services
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


        public async Task<ResultOrErrors<OAuthToken>> SignUpAsync(string username, string email, string password)
        {
            _logger.InitMethod(nameof(SignUpAsync), username, email, "pass");

            var errors = new List<string>();
            if (String.IsNullOrWhiteSpace(username)) { errors.Add(_localizer["Username is empty!"]); }
            if (String.IsNullOrWhiteSpace(email))    { errors.Add(_localizer["Email is empty!"]);    }
            if (String.IsNullOrWhiteSpace(password)) { errors.Add(_localizer["Password is empty!"]); }
            if (errors.Count > 0)
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors<OAuthToken>(errors);
            }

            var now = DateTime.Now;
            var userAccount = new UserAccount { UserName = username, Email = email, RegisteredAt = now, LastOnlineAt = now};
            var result = await _userManager.CreateAsync(userAccount, password); //? check "already exist"?

            if (result.Succeeded)
            {
                var userProfile = await _userProfileRep.AddAsync(new UserProfile
                {
                    Id = userAccount.Id,
                    UserName = userAccount.UserName
                });

                var oAuth = await GetTokenAsync(userAccount);

                await _tokenRep.AddAsync(oAuth, userAccount.Id); //ToDo: DeviceName

                _logger.LogInformation($"successfully created new user with id: {userAccount.Id}, username: {username} and email: {email} | rTokenLength: {oAuth.RefreshToken.Length} | aToken: {oAuth.AccessToken.Length}");

                return new ResultOrErrors<OAuthToken>(oAuth);
            }

            _logger.LogWarning("fail creating");

            return new ResultOrErrors<OAuthToken>(result.Errors.Select(e => e.Description));
        }

        public async Task<ResultOrErrors<OAuthToken>> SignInAsync(string login, string password)
        {
            _logger.InitMethod(nameof(SignInAsync), login, "pass");

            var errors = new List<string>();
            if (String.IsNullOrWhiteSpace(login))    { errors.Add(_localizer["Username/Email is empty!"]); }
            if (String.IsNullOrWhiteSpace(password)) { errors.Add(_localizer["Password is empty!"]);       }
            if (errors.Count > 0)
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors<OAuthToken>(errors);
            }

            var userAccount = await _userManager.FindByEmailOrUserNameAsync(login);
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");

                return new ResultOrErrors<OAuthToken>(_localizer["User with username/email {0} not found!", login]);
            }

            var passValidated = await _userManager.CheckPasswordAsync(userAccount, password);

            if (!passValidated)
            {
                _logger.LogWarning("password failed");

                return new ResultOrErrors<OAuthToken>(_localizer["Invalid login or password!"]);
            }

            var oAuth = await GetTokenAsync(userAccount);

            await _tokenRep.AddAsync(oAuth, userAccount.Id); //ToDo: DeviceName

            _logger.LogInformation($"user successfully signed in | rTokenLength: {oAuth.RefreshToken.Length} | aTokenLength: {oAuth.AccessToken.Length}");

            return new ResultOrErrors<OAuthToken>(oAuth);
        }

        public async Task<ResultOrErrors<OAuthToken>> RefreshTokenAsync(string refreshToken)
        {
            _logger.InitMethod(nameof(RefreshTokenAsync), $"[Length] {refreshToken?.Length}");

            if (String.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors<OAuthToken>(_localizer["Refresh token is Empty!"]);
            }

            var currentToken = await _tokenRep.GetByRefreshAsync(refreshToken);

            if (currentToken == null)
            {
                _logger.LogWarning("refresh token not found");

                return new ResultOrErrors<OAuthToken>(_localizer["Invalid refresh token!"]);
            }

            var lifetime = (long)(TimeSpan.FromDays(int.Parse(_configuration["Token:RefreshLifeTimeInDays"])).TotalMilliseconds);

            var expiredDate = currentToken.CreatedAt + lifetime;

            if (DateTime.Now.Millisecond > expiredDate)
            {
                _logger.LogWarning("refresh token expired");

                return new ResultOrErrors<OAuthToken>(_localizer["Refresh token expired!"]);
            }

            if (currentToken.UserAccount == null) 
            {
                _logger.LogError("user for valid (?!) refresh token not found");

                return new ResultOrErrors<OAuthToken>(_localizer["User for this refresh token not found!"]);
            }

            var newOAuth = await GetTokenAsync(currentToken.UserAccount);

            await _tokenRep.AddAsync(newOAuth, currentToken.UserId); //ToDo: DeviceName
            await _tokenRep.DeleteAsync(currentToken);

            _logger.LogInformation($"successfully refreshed token | rTokenLength: {newOAuth.RefreshToken.Length} | aTokenLength: {newOAuth.AccessToken.Length}");

            return new ResultOrErrors<OAuthToken>(newOAuth);
        }

        public async Task<ResultOrErrors> RevokeTokenAsync(string refreshToken)
        {
            _logger.InitMethod(nameof(RevokeTokenAsync), $"[Length] {refreshToken?.Length}");

            if (String.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors(_localizer["Refresh token is Empty!"]);
            }

            var currentToken = await _tokenRep.GetByRefreshAsync(refreshToken);

            if (currentToken == null)
            {
                _logger.LogWarning("refresh token not found");

                return new ResultOrErrors<OAuthToken>(_localizer["Invalid refresh token!"]);
            }

            await _tokenRep.DeleteAsync(currentToken);

            _logger.LogInformation("token successfully revoked");

            return ResultOrErrors.SuccessResult;
        }

        public async Task<ResultOrErrors> RevokeAllTokensAsync(int userId)
        {
            _logger.InitMethod(nameof(RevokeAllTokensAsync), userId);

            if (userId < 0)
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors(_localizer["Invalid user's id!"]);
            }

            var tokens = await _tokenRep.GetByUserIdAsync(userId);

            await _tokenRep.DeleteAsync(tokens);

            _logger.LogInformation("tokens successfully revoked");

            return ResultOrErrors.SuccessResult;
        }

        public async Task<ResultOrErrors> GenerateValidateTokenForResetPasswordAsync(string email)
        {
            _logger.InitMethod(nameof(GenerateValidateTokenForResetPasswordAsync), email);

            if (String.IsNullOrWhiteSpace(email))
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors(_localizer["Email is required!"]);
            }

            var userAccount = await _userManager.FindByEmailAsync(email);
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                return new ResultOrErrors(_localizer["User with email '{0}' not found!", email]);
            }

            var validateToken = await _userManager.GeneratePasswordResetTokenAsync(userAccount);

            _logger.LogInformation($"validateTokenLength: {validateToken?.Length}");

            if (String.IsNullOrWhiteSpace(validateToken)) { return new ResultOrErrors("Can't generate validate token!"); }

            await _emailSender.SendEmailAsync(new SendEmailOptions
            {
                Email = userAccount.Email,
                Subject = _localizer["Reset password"],
                Body = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.PasswordResetValidateToken, userAccount.UserName, userAccount.Id, validateToken)
            });

            _logger.LogInformation($"validate token sent to mail {userAccount.Email}");

            return ResultOrErrors.SuccessResult;
        }

        public async Task<ResultOrErrors<OAuthToken>> ResetPasswordAsync(int userId, string validateToken, string newPassword)
        {
            _logger.InitMethod(nameof(ResetPasswordAsync), userId, $"[Length] {validateToken?.Length}", "newPass");

            var errors = new List<string>();
            if (userId < 0)                               { errors.Add(_localizer["Invalid user's id!"]); }
            if (String.IsNullOrWhiteSpace(validateToken)) { errors.Add(_localizer["Validate token is empty!"]); }
            if (String.IsNullOrWhiteSpace(newPassword))   { errors.Add(_localizer["New password is empty!"]); }
            if (errors.Count > 0)
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors<OAuthToken>(errors);
            }

            var userAccount = await _userManager.FindByIdAsync(userId.ToString());
            if (userAccount == null)
            {
                _logger.LogWarning($"user not found");
                return new ResultOrErrors<OAuthToken>(_localizer["User with id '{0}' not found!", userId]);
            }

            var result = await _userManager.ResetPasswordAsync(userAccount, validateToken, newPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("password has changed");

                return await RevokeAllTokensAndGetNewAsync(userAccount);
            }

            return new ResultOrErrors<OAuthToken>(result.Errors.Select(e => e.Description));
        }

        public async Task<ResultOrErrors> GenerateValidateTokenForConfirmEmailAsync(int userId)
        {
            _logger.InitMethod(nameof(GenerateValidateTokenForConfirmEmailAsync), userId);

            if (userId < 0)
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors(_localizer["Invalid user's id!"]);
            }

            var userAccount = await _userManager.FindByIdAsync(userId.ToString());
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                return new ResultOrErrors<OAuthToken>(_localizer["User with id '{0}' not found!", userId]);
            }

            if (userAccount.EmailConfirmed)
            {
                _logger.LogWarning("email already confirmed");
                return new ResultOrErrors(_localizer["Email {0} is already confirmed!"]);
            }

            var validateToken = await _userManager.GenerateEmailConfirmationTokenAsync(userAccount);

            _logger.LogInformation($"validateTokenLength: {validateToken?.Length}");

            if (String.IsNullOrWhiteSpace(validateToken)) { return new ResultOrErrors("Can't generate validate token!"); }

            await _emailSender.SendEmailAsync(new SendEmailOptions
            {
                Email   = userAccount.Email,
                Subject = _localizer["Confirm email"],
                Body    = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.ConfirmEmail, userAccount.UserName, userAccount.Id, validateToken)
            });

            _logger.LogInformation($"validate token sent to mail {userAccount.Email}");

            return ResultOrErrors.SuccessResult;
        }

        public async Task<ResultOrErrors> ConfirmEmailAsync(int userId, string validateToken)
        {
            _logger.InitMethod(nameof(ConfirmEmailAsync), userId, $"[Length] {validateToken?.Length}");

            var errors = new List<string>();
            if (userId < 0) { errors.Add(_localizer["Invalid user's id!"]); }
            if (String.IsNullOrWhiteSpace(validateToken)) { errors.Add(_localizer["Validate token is empty!"]); }
            if (errors.Count > 0)
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors<OAuthToken>(errors);
            }

            var userAccount = await _userManager.FindByIdAsync(userId.ToString());
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                return new ResultOrErrors<OAuthToken>(_localizer["User with id '{0}' not found!", userId]);
            }

            var result = await _userManager.ConfirmEmailAsync(userAccount, validateToken); // EmailConfirmed set true inside
            if (result.Succeeded)
            {
                _logger.LogInformation("email successefully confirmed");
                return ResultOrErrors.SuccessResult;
            }

            await _emailSender.SendEmailAsync(new SendEmailOptions
            {
                Email = userAccount.Email,
                Subject = _localizer["Email confirmed"],
                Body = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.EmailConfirmed, userAccount.UserName)
            });

            return new ResultOrErrors(result.Errors.Select(e => e.Description));
        }

        public async Task<ResultOrErrors<OAuthToken>> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            _logger.InitMethod(nameof(ChangePasswordAsync), userId, "currPass", "newPass");

            var errors = new List<string>();
            if (userId < 0) { errors.Add(_localizer["Invalid user's id!"]); }
            if (String.IsNullOrWhiteSpace(currentPassword)) { errors.Add(_localizer["Old password is empty!"]); }
            if (String.IsNullOrWhiteSpace(newPassword)) { errors.Add(_localizer["New password is empty!"]); }
            if (errors.Count > 0)
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors<OAuthToken>(errors);
            }

            var userAccount = await _userManager.FindByIdAsync(userId.ToString());
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                return new ResultOrErrors<OAuthToken>(_localizer["User with id '{0}' not found!", userId]);
            }

            var result = await _userManager.ChangePasswordAsync(userAccount, currentPassword, newPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("password has changed");

                return await RevokeAllTokensAndGetNewAsync(userAccount);
            }

            return new ResultOrErrors<OAuthToken>(result.Errors.Select(e => e.Description));
        }

        public async Task<ResultOrErrors<OAuthToken>> ChangePasswordAsync(int userId, string newPassword)
        {
            _logger.InitMethod(nameof(ChangePasswordAsync), userId, "newPass");

            var errors = new List<string>();
            if (userId < 0)                             { errors.Add(_localizer["Invalid user's id!"]); }
            if (String.IsNullOrWhiteSpace(newPassword)) { errors.Add(_localizer["New password is empty!"]); }
            if (errors.Count > 0)
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors<OAuthToken>(errors);
            }

            var userAccount = await _userManager.FindByIdAsync(userId.ToString());
            if (userAccount == null)
            {
                _logger.LogWarning($"user not found");
                return new ResultOrErrors<OAuthToken>(_localizer["User with id '{0}' not found!", userId]);
            }

            userAccount.PasswordHash = _userManager.PasswordHasher.HashPassword(userAccount, newPassword);

            var result = await _userManager.UpdateAsync(userAccount);

            if (result.Succeeded)
            {
                _logger.LogInformation("password has changed");

                await _emailSender.SendEmailAsync(new SendEmailOptions
                {
                    Email   = userAccount.Email,
                    Subject = _localizer["Password has changed"],
                    Body    = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.ProfileHasChanged, userAccount.UserName, "Password")
                });
                
                return await RevokeAllTokensAndGetNewAsync(userAccount);
            }

            return new ResultOrErrors<OAuthToken>(result.Errors.Select(e => e.Description));
        }

        public async Task<ResultOrErrors> GenerateValidateTokenForChangeEmailAsync(int userId, string newEmail)
        {
            _logger.InitMethod(nameof(GenerateValidateTokenForChangeEmailAsync), userId, newEmail);

            var errors = new List<string>();
            if (userId < 0) { errors.Add(_localizer["Invalid user's id!"]); }
            if (String.IsNullOrWhiteSpace(newEmail)) { errors.Add(_localizer["New email is required!"]); }
            if (errors.Count > 0)
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors(errors);
            }

            var userAccount = await _userManager.FindByIdAsync(userId.ToString());
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                return new ResultOrErrors(_localizer["User with id '{0}' not found!", userId]);
            }

            var validateToken = await _userManager.GenerateChangeEmailTokenAsync(userAccount, newEmail);

            _logger.LogInformation($"validateTokenLength: {validateToken?.Length}");

            if (String.IsNullOrWhiteSpace(validateToken)) { return new ResultOrErrors("Can't generate validate token!"); }

            await _emailSender.SendEmailAsync(new SendEmailOptions
            {
                Email = userAccount.Email,
                Subject = _localizer["Confirm email"],
                Body = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.ConfirmEmail, userAccount.UserName, userAccount.Id, validateToken)
            });

            _logger.LogInformation($"validate token sent to mail {userAccount.Email}");

            return ResultOrErrors.SuccessResult;
        }

        public async Task<ResultOrErrors> ChangeEmailAsync(int userId, string validateToken, string newEmail)
        {
            _logger.InitMethod(nameof(ChangeEmailAsync), userId, newEmail);

            var errors = new List<string>();
            if (userId < 0) { errors.Add(_localizer["Invalid user's id!"]); }
            if (String.IsNullOrWhiteSpace(validateToken)) { errors.Add(_localizer["Validate token is empty!"]); }
            if (String.IsNullOrWhiteSpace(newEmail)) { errors.Add(_localizer["New Email is empty!"]); }
            if (errors.Count > 0)
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors<OAuthToken>(errors);
            }

            var userAccount = await _userManager.FindByIdAsync(userId.ToString());
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                return new ResultOrErrors<OAuthToken>(_localizer["User with id '{0}' not found!", userId]);
            }

            var result = await _userManager.ChangeEmailAsync(userAccount, newEmail, validateToken); // EmailConfirmed set true inside
            if (result.Succeeded)
            {
                _logger.LogInformation("email successefully changed");

                await _emailSender.SendEmailAsync(new SendEmailOptions
                {
                    Email = userAccount.Email,
                    Subject = _localizer["Email has changed"],
                    Body = await _emailBodyBuilder.BuildAsync(StaticData.EmailTemplate.ProfileHasChanged, userAccount.UserName, "Email")
                });

                return ResultOrErrors.SuccessResult;
            }

            return new ResultOrErrors(result.Errors.Select(e => e.Description));
        }

        public async Task<ResultOrErrors> ChangeUserNameAsync(int userId, string newUserName)
        {
            _logger.InitMethod(nameof(ChangeUserNameAsync), userId, newUserName);

            var errors = new List<string>();
            if (userId < 0) { errors.Add(_localizer["Invalid user's id!"]); }
            if (String.IsNullOrWhiteSpace(newUserName)) { errors.Add(_localizer["New username token is empty!"]); }
            if (errors.Count() > 0)
            {
                _logger.SkipInvalidInput();
                return new ResultOrErrors(errors);
            }

            var userAccount = await _userManager.FindByIdAsync(userId.ToString());
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                return new ResultOrErrors<OAuthToken>(_localizer["User with id '{0}' not found!", userId]);
            }

            var result = await _userManager.SetUserNameAsync(userAccount, newUserName);
            if (result.Succeeded)
            {
                _logger.SuccessEndMethod(nameof(ChangeUserNameAsync));
                return ResultOrErrors.SuccessResult;
            }

            return new ResultOrErrors(result.GetErrorDescriptions());
        }


        private async Task<OAuthToken> GetTokenAsync(UserAccount userAccount)
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

        private async Task<ResultOrErrors<OAuthToken>> RevokeAllTokensAndGetNewAsync(UserAccount userAccount)
        {
            _logger.InitMethod(nameof(RevokeAllTokensAndGetNewAsync), $"[UserAccount.Id] {userAccount.Id}");

            var revokeResult = await RevokeAllTokensAsync(userAccount.Id);

            if (revokeResult.Succeeded)
            {
                _logger.LogInformation("all tokens revoked");

                var oAuth = await GetTokenAsync(userAccount);

                await _tokenRep.AddAsync(oAuth, userAccount.Id); //ToDo: DeviceName

                _logger.LogInformation($"new token is created | aTokenLength: {oAuth.AccessToken.Length} | rTokenLength: {oAuth.RefreshToken.Length}");

                return new ResultOrErrors<OAuthToken>(oAuth);
            }

            _logger.LogWarning("token aren't revoked");

            return new ResultOrErrors<OAuthToken>(revokeResult.Errors);
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

        private async Task<ResultOrErrors<UserAccount>> FindUserByIdAsync(int userId) //ToDo: do it better
        {
            _logger.InitMethod(nameof(FindUserByIdAsync), userId);

            var userAccount = await _userManager.FindByIdAsync(userId.ToString());
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                return new ResultOrErrors<UserAccount>(_localizer["User with id '{0}' not found!", userId]);
            }

            _logger.LogInformation($"user found: {userAccount.UserName}");

            return new ResultOrErrors<UserAccount>(userAccount);
        }

        private async Task<ResultOrErrors<UserAccount>> FindUserByLoginAsync(string login) //ToDo: do it better
        {
            _logger.InitMethod(nameof(FindUserByLoginAsync), login);

            var userAccount = await _userManager.FindByEmailOrUserNameAsync(login);
            if (userAccount == null)
            {
                _logger.LogWarning("user not found");
                return new ResultOrErrors<UserAccount>(_localizer["User with username/email {0} not found!", login]);
            }

            _logger.LogInformation($"user found: {userAccount.UserName}");

            return new ResultOrErrors<UserAccount>(userAccount);
        }

        private IEnumerable<string> CheckArgs(IDictionary<string, object> args) //? it's true way?
        {
            var errors = new List<string>();

            foreach (var arg in args)
            {
                if (arg.Value is int)
                {
                    if ((int)arg.Value < 0)
                    {
                        errors.Add(_localizer[$"{arg.Key} is invaid!"]);
                    }
                }
                if (arg.Value is string)
                {
                    if (String.IsNullOrWhiteSpace((string)arg.Value))
                    {
                        errors.Add(_localizer[$"{arg.Key} is empty!"]);
                    }
                }
            }

            if (errors.Count > 0)
            {
                _logger.LogWarning("skip invalid input");
            }

            return errors;
        }
    }
}
