using System;
using System.Collections.Generic;

namespace AuthService.Application.Exceptions
{
    public class ValidationException : AuthException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(string message) : base(message, "VALIDATION_ERROR")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message, Dictionary<string, string[]> errors) 
            : base(message, "VALIDATION_ERROR")
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }

        public ValidationException(string message, Dictionary<string, string[]> errors, Exception innerException) 
            : base(message, "VALIDATION_ERROR", innerException)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }

        public void AddError(string field, string error)
        {
            if (!Errors.ContainsKey(field))
            {
                Errors[field] = new string[0];
            }

            var currentErrors = new List<string>(Errors[field]);
            currentErrors.Add(error);
            Errors[field] = currentErrors.ToArray();
        }

        public void AddErrors(string field, string[] errors)
        {
            if (!Errors.ContainsKey(field))
            {
                Errors[field] = new string[0];
            }

            var currentErrors = new List<string>(Errors[field]);
            currentErrors.AddRange(errors);
            Errors[field] = currentErrors.ToArray();
        }
    }
} 