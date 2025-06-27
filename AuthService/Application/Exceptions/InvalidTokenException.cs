using System;

namespace AuthService.Application.Exceptions
{
    public class InvalidTokenException : AuthException
    {
        public string Token { get; }
        public string Reason { get; }

        public InvalidTokenException(string reason = "Invalid token") 
            : base(reason, "INVALID_TOKEN")
        {
            Reason = reason;
        }

        public InvalidTokenException(string token, string reason) 
            : base(reason, "INVALID_TOKEN")
        {
            Token = token;
            Reason = reason;
        }

        public InvalidTokenException(string reason, Exception innerException) 
            : base(reason, "INVALID_TOKEN", innerException)
        {
            Reason = reason;
        }

        public InvalidTokenException(string token, string reason, Exception innerException) 
            : base(reason, "INVALID_TOKEN", innerException)
        {
            Token = token;
            Reason = reason;
        }
    }
} 