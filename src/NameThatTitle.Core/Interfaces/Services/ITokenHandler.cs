﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using NameThatTitle.Core.Models.Token;

namespace NameThatTitle.Core.Interfaces.Services
{
    public interface ITokenHandler
    {
        OAuthToken Create(string secretKey, int expiresMinutes, IEnumerable<Claim> claims);

        IEnumerable<Claim> GetClaimsFromToken(string accessToken);
    }
}
