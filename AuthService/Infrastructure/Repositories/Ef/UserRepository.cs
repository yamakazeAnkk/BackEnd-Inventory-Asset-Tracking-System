using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using AuthService.Application.Exceptions;

namespace AuthService.Infrastructure.Repositories.Ef
{
    public class UserRepository : IUserRepository 
    {
        private readonly AuthDbContext _context;
        
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(AuthDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User added successfully: {User}", user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {User}", user);
                throw new AuthException("Error adding user", ex);
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                throw new AuthException("Error getting user by email", ex);
            }
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
                if(user == null)
                {
                    throw new UserNotFoundException(userId, "userId");
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by id: {UserId}", userId);
                throw new AuthException("Error getting user by id", ex);
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try 
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                throw new AuthException("Error getting user by username", ex);
            }
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await GetUserByIdAsync(userId);
            try
            {
                var roles = await _context.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.Role.Name).ToListAsync();
                return roles;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error getting user roles: {UserId}", userId);
                throw new AuthException("Error getting user roles", ex);
            }
        }

        public async Task<bool> UserExistsAsync(string email, string username)
        {
            if(string.IsNullOrEmpty(email) && string.IsNullOrEmpty(username)){
                throw new ValidationException("At least one identifier (email or username) must be provided");
            }
            try
            {
                var query = _context.Users.AsNoTracking().AsQueryable();
                if(!string.IsNullOrEmpty(email)){
                    query = query.Where(u => u.Email == email);
                }
                if(!string.IsNullOrEmpty(username)){
                    query = query.Where(u => u.Username == username);
                }
                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while checking user existence. Email: {Email}, Username: {Username}", 
                          email, username);
                throw new AuthException("Failed to verify user existence", ex);
            }
        }
    }
}