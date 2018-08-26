using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NameThatTitle.Domain.Models.Users;

namespace NameThatTitle.Domain.Extensions
{
    public static class UserManagerExtensions
    {
        public async static Task<UserAccount> FindByEmailOrUserNameAsync(this UserManager<UserAccount> userManager, string login)
        {
            return login.Contains('@') ?
                await userManager.FindByEmailAsync(login) :
                await userManager.FindByNameAsync(login);
        }
    }
}
