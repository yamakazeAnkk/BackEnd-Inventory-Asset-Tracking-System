using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Dtos.User;

namespace AuthService.Application.Dtos.Auth
{
    public class RegisterResponse
    {
        public UserReadDto Data { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}