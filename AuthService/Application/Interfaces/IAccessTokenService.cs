using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using System.Security.Claims;

namespace AuthService.Application.Interfaces
{
    public interface IAccessTokenService
    {
        Task<string> GenerateAccessTokenAsync(User user, IList<string> roles);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}