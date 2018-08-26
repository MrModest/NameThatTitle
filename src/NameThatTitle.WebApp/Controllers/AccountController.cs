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
using NameThatTitle.Domain.Interfaces.Repositories;
using NameThatTitle.Domain.Interfaces.Services;
using NameThatTitle.Domain.Extensions;
using NameThatTitle.Domain.Models.Token;
using NameThatTitle.Domain.Models.Users;
using NameThatTitle.WebApp.ViewModels;

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
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            var result = await _accountService.SignUpAsync(model.UserName, model.Email, model.Password);

            if (result.Succeeded)
            {
                return Ok(result.Result);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }

            var result = await _accountService.SignInAsync(model.Login, model.Password);

            if (result.Succeeded)
            {
                return Ok(result.Result);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var result = await _accountService.RefreshTokenAsync(refreshToken);

            if (result.Succeeded)
            {
                return Ok(result.Result);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> RevokeToken(string refreshToken)
        {
            var result = await _accountService.RevokeTokenAsync(refreshToken);

            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        public async Task<IActionResult> RevokeAllTokens()
        {
            var result = await _accountService.RevokeAllTokensAsync(User.GetUserId());

            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string newPassword)
        {
            var result = await _accountService.ChangePasswordAsync(User.GetUserId(), newPassword);

            if (result.Succeeded)
            {
                return Ok(result.Result);
            }

            return BadRequest(result.Errors);
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