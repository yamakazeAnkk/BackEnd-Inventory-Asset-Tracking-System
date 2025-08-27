using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        public Task<string> GenerateRefreshTokenAsync(User user, string ipAddress, string userAgent)
        {
            throw new NotImplementedException();
        }

        public Task RevokeAllUserRefreshTokensAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task RevokeRefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> RotateRefreshTokenAsync(string oldRefreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}