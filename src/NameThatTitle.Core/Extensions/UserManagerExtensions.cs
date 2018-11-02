using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NameThatTitle.Core.Models.Users;

namespace NameThatTitle.Core.Extensions
{
    public static class UserManagerExtensions //ToDo: replace it to separate project: 'NameThatTitle.Commons'
    {
        public static async Task<UserAccount> FindByEmailOrUserNameAsync(this UserManager<UserAccount> userManager, string login)
        {
            return login.Contains('@') ?
                await userManager.FindByEmailAsync(login) :
                await userManager.FindByNameAsync(login);
        }
    }
}
