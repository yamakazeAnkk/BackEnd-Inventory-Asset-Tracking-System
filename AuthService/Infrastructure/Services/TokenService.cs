using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using AuthService.Application.Exceptions;
using AuthService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<TokenService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;

        public TokenService(IOptions<JwtOptions> jwtOptions, ILogger<TokenService> logger, IUserRepository userRepository, IAuthRepository authRepository)
        {
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            _userRepository = userRepository;
            _authRepository = authRepository;
        }

        public async Task<string> GenerateRefreshTokenAsync(User user)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var refreshToken = Convert.ToBase64String(randomBytes);

            var existingRefreshToken = await _authRepository.GetLatestRefreshTokenForUserAsync(user.Id);
            if (existingRefreshToken != null)
            {
                if(existingRefreshToken.ExpiresAt < DateTime.UtcNow){
                    existingRefreshToken.RevokedAt = DateTime.UtcNow;
                    await _authRepository.UpdateRefreshTokenAsync(existingRefreshToken);
                }
                else{
                    existingRefreshToken.Token = refreshToken;
                    existingRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
                    existingRefreshToken.RevokedAt = null;
                    existingRefreshToken.CreatedAt = DateTime.UtcNow;
                    await _authRepository.UpdateRefreshTokenAsync(existingRefreshToken);    
                }
            }
            else
            {
                var newRefreshToken = new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow
                };
                await _authRepository.SaveRefreshTokenAsync(newRefreshToken);
            }
            return refreshToken;
        }

        public async Task<string> GenerateAccessTokenAsync(User user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            var userClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fullName", user.FullName),
                new Claim("isActive", user.IsActive.ToString())
            };
            foreach(var role in roles){
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false, // allow expired
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidAudience = _jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey))
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (!(securityToken is JwtSecurityToken jwtToken) || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new InvalidTokenException("Invalid token format or algorithm");
                return principal;
            }
            catch (SecurityTokenException ex)
            {
                throw new InvalidTokenException("Invalid or expired token", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidTokenException("Token validation failed", ex);
            }
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
            }, out SecurityToken validatedToken);
            return Task.FromResult(true);
        }

        public Task RevokeTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
            }, out SecurityToken validatedToken);
            return Task.FromResult(true);
        }

        // Missing function blacklist token
        public Task RevokeAllTokensAsync(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            tokenHandler.ValidateToken(userId, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
            }, out SecurityToken validatedToken);
            return Task.FromResult(true);
        }

        public async Task<string> RefreshAccessTokenAsync(string refreshToken)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(refreshToken);
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if(userId == null){
                    return string.Empty;
                }
                var user = await _userRepository.GetUserByIdAsync(userId);
                var roles = await _userRepository.GetUserRolesAsync(user.Id.ToString());
                var newAccessToken = await GenerateAccessTokenAsync(user, roles);
                return newAccessToken;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error refreshing access token: {RefreshToken}", refreshToken);
                throw new InvalidTokenException("Error refreshing access token", ex);
            }
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Get refresh token from database
                var refreshTokenEntity = await _authRepository.GetRefreshTokenAsync(refreshToken);
                if (refreshTokenEntity == null)
                {
                    return false;
                }

                // Check if token is expired
                if (refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
                {
                    return false;
                }

                // Check if token is revoked
                if (refreshTokenEntity.RevokedAt.HasValue)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating refresh token");
                return false;
            }
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var refreshTokenEntity = await _authRepository.GetRefreshTokenAsync(refreshToken);
                if (refreshTokenEntity != null)
                {
                    refreshTokenEntity.RevokedAt = DateTime.UtcNow;
                    await _authRepository.UpdateRefreshTokenAsync(refreshTokenEntity);
                    _logger.LogInformation("Refresh token revoked: {RefreshToken}", refreshToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token: {RefreshToken}", refreshToken);
                throw;
            }
        }

        public async Task RevokeAllUserRefreshTokensAsync(Guid userId)
        {
            try
            {
                await _authRepository.RevokeAllUserRefreshTokensAsync(userId);
                _logger.LogInformation("All refresh tokens revoked for user: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all refresh tokens for user: {UserId}", userId);
                throw;
            }
        }
        
    }
}