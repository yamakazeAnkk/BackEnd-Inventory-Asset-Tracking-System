using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using System.Security.Claims;

namespace AuthService.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(User user, IList<string> roles);
        Task<string> GenerateRefreshTokenAsync();
        Task<bool> ValidateTokenAsync(string token);

        Task RevokeTokenAsync(string token);

        Task RevokeAllTokensAsync(string userId);

        Task<string> RefreshAccessTokenAsync(string refreshToken);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}