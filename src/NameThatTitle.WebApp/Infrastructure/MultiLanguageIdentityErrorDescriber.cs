using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NameThatTitle.WebApp.Infrastructure
{
    public class MultiLanguageIdentityErrorDescriber : IdentityErrorDescriber
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public MultiLanguageIdentityErrorDescriber(IStringLocalizer<SharedResource> localizer) //? SharedResource - empty class is enough?
        {
            _localizer = localizer;
        }

        public override IdentityError ConcurrencyFailure()
        {
            return Localize(base.ConcurrencyFailure());
        }

        public override IdentityError DefaultError()
        {
            return Localize(base.DefaultError());
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return Localize(base.DuplicateEmail(email), email);
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return Localize(base.DuplicateRoleName(role), role);
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return Localize(base.DuplicateUserName(userName), userName);
        }

        public override IdentityError InvalidEmail(string email)
        {
            return Localize(base.InvalidEmail(email), email);
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return Localize(base.InvalidRoleName(role), role);
        }

        public override IdentityError InvalidToken()
        {
            return Localize(base.InvalidToken());
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return Localize(base.InvalidUserName(userName), userName);
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return Localize(base.LoginAlreadyAssociated());
        }

        public override IdentityError PasswordMismatch()
        {
            return Localize(base.PasswordMismatch());
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return Localize(base.PasswordRequiresDigit());
        }

        public override IdentityError PasswordRequiresLower()
        {
            return Localize(base.PasswordRequiresLower());
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return Localize(base.PasswordRequiresNonAlphanumeric());
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return Localize(base.PasswordRequiresUniqueChars(uniqueChars), uniqueChars);
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return Localize(base.PasswordRequiresUpper());
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return Localize(base.PasswordTooShort(length), length);
        }

        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return Localize(base.RecoveryCodeRedemptionFailed());
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return Localize(base.UserAlreadyHasPassword());
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return Localize(base.UserAlreadyInRole(role), role);
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return Localize(base.UserLockoutNotEnabled());
        }

        public override IdentityError UserNotInRole(string role)
        {
            return Localize(base.UserNotInRole(role), role);
        }


        private IdentityError Localize(IdentityError ie)
        {
            ie.Description = _localizer[ie.Description];
            return ie;
        }

        private IdentityError Localize(IdentityError ie, object arg)
        {
            ie.Description = _localizer[ie.Description, arg];
            return ie;
        }
    }
}
