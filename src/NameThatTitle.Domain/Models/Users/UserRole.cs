using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace NameThatTitle.Domain.Models.Users
{
    public class UserRole : IdentityRole<int>
    {
        //public string Description { get; set; } //! MultiLanguage
    }
}
