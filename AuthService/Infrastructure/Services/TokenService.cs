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

namespace AuthService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<TokenService> _logger;
        private readonly IUserRepository _userRepository;

        public TokenService(IOptions<JwtOptions> jwtOptions, ILogger<TokenService> logger, IUserRepository userRepository)
        {
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<string> GenerateRefreshTokenAsync()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
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

        
    }
}