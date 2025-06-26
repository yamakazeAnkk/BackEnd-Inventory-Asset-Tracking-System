using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                if(await UserExistsAsync(user.Email, user.Username))
                {
                    throw new Exception("User already exists");
                }
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User added successfully: {User}", user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {User}", user);
                throw new Exception("Error adding user");
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if(user == null)
                {
                    throw new Exception("User not found");
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                throw new Exception("Error getting user by email");
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try 
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if(user == null)
                {
                    throw new Exception("User not found");
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                throw new Exception("Error getting user by username");
            }
        }
        public async Task<bool> UserExistsAsync(string email, string username)
        {
            if(string.IsNullOrEmpty(email) && string.IsNullOrEmpty(username)){
                        throw new ArgumentException("At least one identifier (email or username) must be provided");
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
                throw new Exception("Failed to verify user existence");
            }
        }
    }
}