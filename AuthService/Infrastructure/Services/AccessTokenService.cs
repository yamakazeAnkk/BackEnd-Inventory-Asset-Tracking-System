    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AuthService.Application.Interfaces;
    using AuthService.Domain.Entities;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using AuthService.Infrastructure.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;
    using System.IdentityModel.Tokens.Jwt;
    using AuthService.Configuration;
    using AuthService.Application.Exceptions;

    namespace AuthService.Infrastructure.Services
    {
        public class AccessTokenService : IAccessTokenService
        {
            private readonly ILogger<AccessTokenService> _logger;
            private readonly IUserRepository _userRepository;
            private readonly IAuthRepository _authRepository;
            private readonly JwtOptions _jwtOptions;
            public AccessTokenService(ILogger<AccessTokenService> logger, IUserRepository userRepository, IAuthRepository authRepository, IOptions<JwtOptions> jwtOptions)
            {
                _logger = logger;
                _userRepository = userRepository;
                _authRepository = authRepository;
                _jwtOptions = jwtOptions.Value;
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
        }
    
    
    }