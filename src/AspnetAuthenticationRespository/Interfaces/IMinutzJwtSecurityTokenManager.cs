﻿using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AspnetAuthenticationRepository.Interfaces
{
    public interface IMinutzJwtSecurityTokenManager
    {
        (string token, DateTime expires) JwtSecurityToken
            (string clientSecret, string domain, IEnumerable<Claim> claims);
    }
}