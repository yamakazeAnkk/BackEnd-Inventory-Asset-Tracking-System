using System;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
    }
} 