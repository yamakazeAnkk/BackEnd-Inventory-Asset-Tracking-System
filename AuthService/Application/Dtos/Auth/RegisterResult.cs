using System;
using AuthService.Application.Dtos.User;

namespace AuthService.Application.Dtos.Auth
{
    public class RegisterResult
    {
        public UserReadDto User { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
} 