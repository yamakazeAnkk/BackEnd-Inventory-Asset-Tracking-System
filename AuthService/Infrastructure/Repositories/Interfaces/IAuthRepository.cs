using System;
using System.Threading.Tasks;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> UserExistsAsync(string email, string username);
        Task AddUserAsync(User user);
        Task AddLoginHistoryAsync(UserLoginHistory loginHistory);
        Task<bool> ValidateUserCredentialsAsync(string email, string passwordHash);
        Task UpdateUserAsync(User user);

    }
} 