using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Dtos.Auth;
using AuthService.Application.Dtos.Auth;
using AuthService.Application.Dtos.User;

namespace AuthService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LogoutAsync(string userId);
        Task<LoginResponse> GetUserByIdAsync(string userId);
        Task<LoginResponse> GetUserByEmailAsync(string email);
        Task<LoginResponse> GetUserByUsernameAsync(string username);
        Task<LoginResponse> GetUserByPhoneAsync(string phone);
    }
}