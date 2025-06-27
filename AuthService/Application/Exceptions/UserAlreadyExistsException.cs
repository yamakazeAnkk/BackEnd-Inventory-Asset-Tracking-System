using System;

namespace AuthService.Application.Exceptions
{
    public class UserAlreadyExistsException : AuthException
    {
        public string Email { get; }
        public string Username { get; }

        public UserAlreadyExistsException(string email = null, string username = null) 
            : base("User already exists", "USER_ALREADY_EXISTS")
        {
            Email = email;
            Username = username;
        }

        public UserAlreadyExistsException(string email, string username, Exception innerException) 
            : base("User already exists", "USER_ALREADY_EXISTS", innerException)
        {
            Email = email;
            Username = username;
        }

        public override string Message
        {
            get
            {
                var message = "User already exists";
                if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Username))
                {
                    message += $" with email: {Email} or username: {Username}";
                }
                else if (!string.IsNullOrEmpty(Email))
                {
                    message += $" with email: {Email}";
                }
                else if (!string.IsNullOrEmpty(Username))
                {
                    message += $" with username: {Username}";
                }
                return message;
            }
        }
    }
} 