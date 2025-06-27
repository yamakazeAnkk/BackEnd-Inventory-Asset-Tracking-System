using System;

namespace AuthService.Application.Exceptions
{
    public class InvalidCredentialsException : AuthException
    {
        public string Email { get; }

        public InvalidCredentialsException(string email) 
            : base("Invalid email or password", "INVALID_CREDENTIALS")
        {
            Email = email;
        }

        public InvalidCredentialsException(string email, Exception innerException) 
            : base("Invalid email or password", "INVALID_CREDENTIALS", innerException)
        {
            Email = email;
        }
    }
} 