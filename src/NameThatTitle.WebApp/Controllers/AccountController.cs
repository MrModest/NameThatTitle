using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NameThatTitle.Core.Interfaces.Repositories;
using NameThatTitle.Core.Interfaces.Services;
using NameThatTitle.Core.Extensions;
using NameThatTitle.Core.Models.Token;
using NameThatTitle.Core.Models.Users;
using NameThatTitle.WebApp.ViewModels;
using NameThatTitle.Core.Models;

namespace NameThatTitle.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        private readonly UserManager<UserAccount> _userManager;
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly ITokenHandler _tokenHandler;
        private readonly IConfiguration _configuration;
        private readonly IAsyncRepository<UserProfile> _profileRep;
        private readonly IAccountService _accountService;

        public AccountController(
            ILogger<AccountController> logger,

            UserManager<UserAccount> userManager,
            SignInManager<UserAccount> signInManager,
            ITokenHandler tokenHandler,
            IConfiguration configuration,
            IAsyncRepository<UserProfile> profileRep,
            IAccountService accountService)
        {
            _logger = logger;

            _userManager = userManager;
            _signInManager = signInManager;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
            _profileRep = profileRep;
            _accountService = accountService;

            //? this.userManager.UserValidators.Add(...) - how add to global?
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(modelErrors);
            }

            var (oAuthToken, errors) = (await _accountService.SignUpAsync(model.UserName, model.Email, model.Password)).AsTuple();
            if (!errors.IsNullOrEmpty())
            {
                return BadRequest(errors); 
            }

            return Ok(oAuthToken);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(modelErrors);
            }

            var (oAuthToken, errors) = (await _accountService.SignInAsync(model.Login, model.Password)).AsTuple();
            if (!errors.IsNullOrEmpty())
            {
                return BadRequest(errors);
            }

            return Ok(oAuthToken);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var (oAuthToken, errors) = (await _accountService.RefreshTokenAsync(refreshToken)).AsTuple();
            if (!errors.IsNullOrEmpty())
            {
                return BadRequest(errors);
            }

            return Ok(oAuthToken);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> RevokeToken(string refreshToken)
        {
            var (_, errors) = (await _accountService.RevokeTokenAsync(refreshToken)).AsTuple();
            if (!errors.IsNullOrEmpty())
            {
                return BadRequest(errors);
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> RevokeAllTokens()
        {
            var (_, errors) = (await _accountService.RevokeAllTokensAsync(User.GetUserId())).AsTuple();
            if (!errors.IsNullOrEmpty())
            {
                return BadRequest(errors);
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string newPassword)
        {
            var (oAuthToken, errors) = (await _accountService.ChangePasswordAsync(User.GetUserId(), newPassword)).AsTuple();
            if (!errors.IsNullOrEmpty())
            {
                return BadRequest(errors);
            }

            return Ok(oAuthToken);
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            return Ok(new { message = "Test api request." });
        }

        [HttpGet("[action]")]
        public IActionResult AuthTest()
        {
            return Ok(new { message = "Test auth." });
        }

        [HttpGet("[action]")]
        public IActionResult Claims()
        {
            var claims = User.Claims;

            if (claims == null)
            {
                return Ok(Array.Empty<Claim>());
            }

            return Ok(claims.ToJson());
        }
    }
}