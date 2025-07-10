using System;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AuthService.Infrastructure.Repositories.Ef
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDbContext _context;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(AuthDbContext context, ILogger<AuthRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                throw;
            }
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                throw;
            }
        }

        public async Task<bool> UserExistsAsync(string email, string username)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.Email == email || u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists: {Email} or {Username}", email, username);
                throw;
            }
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User added successfully: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {User}", user);
                throw;
            }
        }

        public async Task AddLoginHistoryAsync(UserLoginHistory loginHistory)
        {
            try
            {
                _context.UserLoginHistories.Add(loginHistory);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Login history added for user: {UserId}", loginHistory.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding login history for user: {UserId}", loginHistory.UserId);
                throw;
            }
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string passwordHash)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user != null && user.PasswordHash == passwordHash && user.IsActive;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user credentials for email: {Email}", email);
                throw;
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                user.UpdatedAt = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User updated successfully: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", user.Id);
                throw;
            }
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            try
            {
                _context.RefreshTokens.Add(refreshToken);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Refresh token saved for user: {UserId}", refreshToken.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving refresh token for user: {UserId}", refreshToken.UserId);
                throw;
            }
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            try
            {
                return await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refresh token: {Token}", token);
                throw;
            }
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            try
            {
                _context.RefreshTokens.Update(refreshToken);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Refresh token updated: {Token}", refreshToken.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating refresh token: {Token}", refreshToken.Token);
                throw;
            }
        }

        public async Task RevokeAllUserRefreshTokensAsync(Guid userId)
        {
            try
            {
                var refreshTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                    .ToListAsync();

                foreach (var token in refreshTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("All refresh tokens revoked for user: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all refresh tokens for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<RefreshToken?> GetLatestRefreshTokenForUserAsync(Guid userId)
        {
            try
            {
                return await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                    .OrderByDescending(rt => rt.CreatedAt)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest refresh token for user: {UserId}", userId);
                throw;
            }
        }
    }
} 