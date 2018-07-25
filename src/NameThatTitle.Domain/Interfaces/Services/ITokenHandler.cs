using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using NameThatTitle.Domain.Models.Token;

namespace NameThatTitle.Domain.Interfaces.Services
{
    public interface ITokenHandler
    {
        OAuthToken Create(string secretKey, int expiresMinutes, IEnumerable<Claim> claims);
    }
}
