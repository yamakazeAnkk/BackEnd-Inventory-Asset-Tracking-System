using System;

namespace AuthService.Application.Exceptions
{
    public class UserNotFoundException : AuthException
    {
        public string Identifier { get; }
        public string IdentifierType { get; }

        public UserNotFoundException(string identifier, string identifierType = "email") 
            : base($"User not found with {identifierType}: {identifier}", "USER_NOT_FOUND")
        {
            Identifier = identifier;
            IdentifierType = identifierType;
        }

        public UserNotFoundException(string identifier, string identifierType, Exception innerException) 
            : base($"User not found with {identifierType}: {identifier}", "USER_NOT_FOUND", innerException)
        {
            Identifier = identifier;
            IdentifierType = identifierType;
        }
    }
} 