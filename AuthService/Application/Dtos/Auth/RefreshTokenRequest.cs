using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Application.Dtos.Auth
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
        
    }
}