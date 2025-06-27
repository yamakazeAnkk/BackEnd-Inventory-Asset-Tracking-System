# Custom Exception Handling System

This document describes the custom exception handling system implemented in the AuthService.

## Overview

The custom exception system provides domain-specific exceptions that are more meaningful than generic exceptions and include proper error codes for better error handling and debugging.

## Exception Hierarchy

```
AuthException (Base)
├── UserNotFoundException
├── InvalidCredentialsException
├── UserAlreadyExistsException
├── InvalidTokenException
└── ValidationException
```

## Exception Classes

### AuthException (Base Class)
Base exception for all authentication-related errors.

**Properties:**
- `ErrorCode`: A string identifier for the error type
- `Message`: The error message

**Usage:**
```csharp
throw new AuthException("Custom error message");
throw new AuthException("Custom error message", "CUSTOM_ERROR_CODE");
throw new AuthException("Custom error message", innerException);
```

### UserNotFoundException
Thrown when a user cannot be found by email, username, or other identifier.

**Properties:**
- `Identifier`: The value used to search for the user
- `IdentifierType`: The type of identifier (email, username, etc.)

**Usage:**
```csharp
throw new UserNotFoundException("user@example.com", "email");
throw new UserNotFoundException("john_doe", "username");
```

### InvalidCredentialsException
Thrown when login credentials are invalid.

**Properties:**
- `Email`: The email address used in the login attempt

**Usage:**
```csharp
throw new InvalidCredentialsException("user@example.com");
```

### UserAlreadyExistsException
Thrown when attempting to register a user that already exists.

**Properties:**
- `Email`: The email that already exists (optional)
- `Username`: The username that already exists (optional)

**Usage:**
```csharp
throw new UserAlreadyExistsException("user@example.com", "john_doe");
throw new UserAlreadyExistsException("user@example.com"); // email only
throw new UserAlreadyExistsException(null, "john_doe"); // username only
```

### InvalidTokenException
Thrown when a JWT token is invalid, expired, or malformed.

**Properties:**
- `Token`: The invalid token (optional)
- `Reason`: The reason for the token being invalid

**Usage:**
```csharp
throw new InvalidTokenException("Token has expired");
throw new InvalidTokenException(token, "Invalid signature");
```

### ValidationException
Thrown when input validation fails.

**Properties:**
- `Errors`: Dictionary of field names and their validation errors

**Usage:**
```csharp
var validationException = new ValidationException("Validation failed");
validationException.AddError("Email", "Email is required");
validationException.AddError("Password", "Password must be at least 8 characters");
throw validationException;
```

## Global Exception Handler

The `GlobalExceptionHandler` middleware automatically catches all exceptions and returns appropriate HTTP status codes and JSON responses.

### HTTP Status Code Mapping

| Exception Type | HTTP Status Code |
|----------------|------------------|
| ValidationException | 400 Bad Request |
| UserNotFoundException | 404 Not Found |
| InvalidCredentialsException | 401 Unauthorized |
| UserAlreadyExistsException | 409 Conflict |
| InvalidTokenException | 401 Unauthorized |
| AuthException | 400 Bad Request |
| Other exceptions | 500 Internal Server Error |

### Response Format

All error responses follow this JSON format:

```json
{
  "statusCode": 400,
  "message": "User not found with email: user@example.com",
  "errorCode": "USER_NOT_FOUND",
  "errors": {
    "email": ["Email is required"],
    "password": ["Password must be at least 8 characters"]
  },
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

## Usage Examples

### In Service Layer

```csharp
public async Task<LoginResponse> LoginAsync(LoginRequest request)
{
    var user = await _userRepository.GetUserByEmailAsync(request.Email);
    if (user == null)
    {
        throw new InvalidCredentialsException(request.Email);
    }
    
    if (!BCrypt.Verify(request.Password, user.PasswordHash))
    {
        throw new InvalidCredentialsException(request.Email);
    }
    
    // ... rest of login logic
}
```

### In Repository Layer

```csharp
public async Task<bool> UserExistsAsync(string email, string username)
{
    if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(username))
    {
        throw new ValidationException("At least one identifier (email or username) must be provided");
    }
    
    // ... database query logic
}
```

### In Controller Layer

Controllers don't need to handle exceptions manually - the global exception handler will catch them and return appropriate responses.

```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    // No try-catch needed - global handler will catch exceptions
    var response = await _authService.LoginAsync(request);
    return Ok(response);
}
```

## Error Codes

| Error Code | Description |
|------------|-------------|
| AUTH_ERROR | Generic authentication error |
| USER_NOT_FOUND | User not found |
| INVALID_CREDENTIALS | Invalid login credentials |
| USER_ALREADY_EXISTS | User already exists during registration |
| INVALID_TOKEN | Invalid or expired JWT token |
| VALIDATION_ERROR | Input validation failed |
| INTERNAL_SERVER_ERROR | Unexpected server error |

## Best Practices

1. **Use specific exceptions**: Always use the most specific exception type available.
2. **Include context**: Provide meaningful error messages and include relevant data.
3. **Log exceptions**: The global handler automatically logs all exceptions.
4. **Don't catch in controllers**: Let the global handler manage exception responses.
5. **Use error codes**: Include error codes for better client-side error handling.
6. **Validate early**: Throw validation exceptions as early as possible in the request pipeline.

## Testing

When testing, you can verify that the correct exceptions are thrown:

```csharp
[Test]
public async Task Login_WithInvalidEmail_ThrowsInvalidCredentialsException()
{
    // Arrange
    var request = new LoginRequest { Email = "nonexistent@example.com", Password = "password" };
    
    // Act & Assert
    var exception = Assert.ThrowsAsync<InvalidCredentialsException>(
        async () => await _authService.LoginAsync(request));
    
    Assert.AreEqual("nonexistent@example.com", exception.Email);
}
``` 