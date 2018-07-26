using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NameThatTitle.Domain.Interfaces.Repositories;
using NameThatTitle.Domain.Interfaces.Services;
using NameThatTitle.Domain.Models;
using NameThatTitle.Domain.Models.Token;
using NameThatTitle.Domain.Models.Users;
using NameThatTitle.Domain.Utils;

namespace NameThatTitle.Domain.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;

        private readonly UserManager<UserAccount> _userManager;
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly ITokenHandler _tokenHandler;
        private readonly IConfiguration _configuration;
        private readonly IAsyncRepository<UserProfile> _userProfileRep;
        private readonly IAsyncRefreshTokenRepository _tokenRep;

        public AccountService(
            UserManager<UserAccount> userManager,
            SignInManager<UserAccount> signInManager,
            ITokenHandler tokenHandler,
            IConfiguration configuration,
            IAsyncRepository<UserProfile> userProfileRep,
            IAsyncRefreshTokenRepository refreshTokenRep,
            ILogger<AccountService> logger)
        {
            _logger = logger;

            _userManager = userManager;
            _signInManager = signInManager;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
            _userProfileRep = userProfileRep;
            _tokenRep = refreshTokenRep;
        }


        public async Task<ResultOrErrors<OAuthToken>> SignUpAsync(string username, string email, string password)
        {
            _logger.LogInformation($"init create new user with username: {username} and email: {email}");

            var errors = new List<string>();

            if (String.IsNullOrEmpty(username)) { errors.Add("Username is empty!");  }
            if (String.IsNullOrEmpty(email)) { errors.Add("Email is empty!"); }
            if (String.IsNullOrEmpty(password)) { errors.Add("Password is empty!"); }

            if (errors.Count > 0)
            {
                _logger.LogWarning("skip empty input");

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

                await _tokenRep.AddAsync(new RefreshToken
                {
                    Refresh = oAuth.RefreshToken,
                    Access = oAuth.AccessToken,
                    CreatedAt = oAuth.CreatedAt,
                    UserId = userAccount.Id
                });

                _logger.LogInformation($"successfully created new user with id: {userAccount.Id}, username: {username} and email: {email} | rTokenLength: {oAuth.RefreshToken.Length} | aToken: {oAuth.AccessToken.Length}");

                return new ResultOrErrors<OAuthToken>(oAuth);
            }

            _logger.LogWarning("fail creating");

            return new ResultOrErrors<OAuthToken>(result.Errors.Select(e => e.Description));
        }

        public async Task<ResultOrErrors<OAuthToken>> SignInAsync(string login, string password)
        {
            _logger.LogInformation($"init sing in user with login: {login}");

            var errors = new List<string>();

            if (String.IsNullOrEmpty(login)) { errors.Add("Username/Email is empty!"); }
            if (String.IsNullOrEmpty(password)) { errors.Add("Password is empty!"); }

            if (errors.Count > 0)
            {
                _logger.LogWarning("skip empty input");

                return new ResultOrErrors<OAuthToken>(errors);
            }

            var userAccount = login.Contains('@') ?
                await _userManager.FindByEmailAsync(login) :
                await _userManager.FindByNameAsync(login);

            if (userAccount == null)
            {
                _logger.LogWarning("user not found");

                return new ResultOrErrors<OAuthToken>($"User with username/email {login} not found!");
            }

            var passValided = await _userManager.CheckPasswordAsync(userAccount, password);

            if (!passValided)
            {
                _logger.LogWarning("password failed");

                return new ResultOrErrors<OAuthToken>($"Invalid login or password!");
            }

            var oAuth = await GetTokenAsync(userAccount);

            await _tokenRep.AddAsync(new RefreshToken //ToDo: DeviceName
            {
                Refresh = oAuth.RefreshToken,
                Access = oAuth.AccessToken,
                CreatedAt = oAuth.CreatedAt,
                UserId = userAccount.Id
            });

            _logger.LogInformation($"user successfully signed in | rTokenLength: {oAuth.RefreshToken.Length} | aTokenLength: {oAuth.AccessToken.Length}");

            return new ResultOrErrors<OAuthToken>(oAuth);
        }

        public async Task<ResultOrErrors<OAuthToken>> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation($"init refresh token | rTokenLength: {refreshToken.Length}");

            if (String.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("skip empty input");

                return new ResultOrErrors<OAuthToken>("Refresh token is Empty!");
            }

            var currentToken = await _tokenRep.GetByRefreshAsync(refreshToken);

            if (currentToken == null)
            {
                _logger.LogWarning("refresh token not found");

                return new ResultOrErrors<OAuthToken>("Invalid token!");
            }

            var lifetime = (long)(TimeSpan.FromDays(int.Parse(_configuration["Jwt:RefreshLifeTimeInDays"])).TotalMilliseconds);

            var expiredDate = currentToken.CreatedAt + lifetime;

            if (DateTime.Now.Millisecond > expiredDate)
            {
                _logger.LogWarning("refresh token expired");

                return new ResultOrErrors<OAuthToken>("Token expired!");
            }

            //? Check `currentToken.UserAccount`
            //var userAccount = await _userManager.FindByIdAsync(currentToken.UserId.ToString());

            //if (userAccount == null)
            //{
            //    return new ResultOrErrors<OAuthToken>("User for this token not found!");
            //}

            //var newOAuth = await GetTokenAsync(userAccount);

            if (currentToken.UserAccount == null) //? EF association works?
            {
                _logger.LogError("user for valid (?!) refresh token not found");

                return new ResultOrErrors<OAuthToken>("User for this token not found!");
            }

            var newOAuth = await GetTokenAsync(currentToken.UserAccount);

            await _tokenRep.AddAsync(new RefreshToken //ToDo: DeviceName
            {
                Refresh = newOAuth.RefreshToken,
                Access = newOAuth.AccessToken,
                CreatedAt = newOAuth.CreatedAt,
                UserId = currentToken.UserId
            });
            await _tokenRep.DeleteAsync(currentToken);

            _logger.LogInformation($"successfully refreshed token | rTokenLength: {newOAuth.RefreshToken.Length} | aTokenLength: {newOAuth.AccessToken.Length}");

            return new ResultOrErrors<OAuthToken>(newOAuth);
        }

        public async Task<ResultOrErrors> RevokeTokenAsync(string refreshToken)
        {
            _logger.LogInformation($"init revoke refresh token {refreshToken}");

            var userToken = await _tokenRep.GetByRefreshAsync(refreshToken);

            if (userToken == null)
            {
                _logger.LogWarning("refresh token not found");

                return new ResultOrErrors<OAuthToken>("Invalid token!");
            }

            await _tokenRep.DeleteAsync(userToken);

            _logger.LogInformation("token successfully revoked");

            return new ResultOrErrors(true);
        }

        private async Task<OAuthToken> GetTokenAsync(UserAccount userAccount)
        {
            _logger.LogInformation($"creating token for user {userAccount.Id}: {userAccount.UserName}");

            var principal = await _signInManager.CreateUserPrincipalAsync(userAccount); //ToDo: avatar, languages

            var identity = (ClaimsIdentity)principal.Identity;

            _logger.LogInformation("user claims: " + LoggingUtils.ClaimsToJson(identity?.Claims));

            if (identity == null)
            {
                return null;
            }

            var token = _tokenHandler.Create(_configuration["Jwt:Key"], int.Parse(_configuration["Jwt:LifeTimeInMinutes"]), identity.Claims);

            _logger.LogInformation($"token successfully created | rToken: {token.RefreshToken.Length} | aTokenLength: {token.AccessToken.Length}");

            return token;
        }
    }
}
