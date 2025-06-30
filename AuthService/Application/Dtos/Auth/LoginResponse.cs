using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Dtos.User;

namespace AuthService.Application.Dtos.Auth
{
public class LoginResponse
    {
        public UserReadDto Data { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}