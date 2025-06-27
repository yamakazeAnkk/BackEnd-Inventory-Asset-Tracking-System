using System;

namespace AuthService.Application.Exceptions
{
    public class AuthException : Exception
    {
        public string ErrorCode { get; }

        public AuthException(string message) : base(message)
        {
            ErrorCode = "AUTH_ERROR";
        }

        public AuthException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public AuthException(string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = "AUTH_ERROR";
        }

        public AuthException(string message, string errorCode, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
} 