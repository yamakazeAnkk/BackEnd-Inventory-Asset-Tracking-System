# API Exception Filter System

This document describes the API exception filter system that provides granular control over exception handling at the controller and action level.

## Overview

The API exception filter system provides two levels of exception handling:

1. **Basic ApiExceptionFilter** - Simple exception handling with standard error responses
2. **AdvancedApiExceptionFilter** - Enhanced exception handling with detailed metadata and debugging information

## Available Filters

### 1. ApiExceptionFilter

A basic exception filter that handles custom exceptions and returns standardized error responses.

**Features:**
- Handles all custom exceptions (AuthException, UserNotFoundException, etc.)
- Returns appropriate HTTP status codes
- Provides structured JSON error responses
- Logs exceptions with controller and action information

**Usage:**
```csharp
[ApiController]
[Route("api/[controller]")]
[ApiExceptionFilter] // Apply to entire controller
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    [ApiExceptionFilter] // Or apply to specific action
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }
}
```

### 2. AdvancedApiExceptionFilter

An enhanced exception filter that provides detailed error information for debugging and monitoring.

**Features:**
- All features of the basic filter
- Request ID tracking for debugging
- Controller and action information
- Detailed error metadata
- Security-conscious stack trace handling
- Enhanced logging with request context

**Usage:**
```csharp
[ApiController]
[Route("api/[controller]")]
[AdvancedApiExceptionFilter] // Apply to entire controller
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    [AdvancedApiExceptionFilter] // Or apply to specific action
    public async Task<IActionResult> GetUser(string id)
    {
        var response = await _authService.GetUserByIdAsync(id);
        return Ok(response);
    }
}
```

## Response Formats

### Basic ApiExceptionFilter Response

```json
{
  "statusCode": 404,
  "message": "User not found with email: user@example.com",
  "errorCode": "USER_NOT_FOUND",
  "errors": {
    "email": ["Email is required"]
  },
  "timestamp": "2024-01-15T10:30:00.000Z"
}
```

### AdvancedApiExceptionFilter Response

```json
{
  "statusCode": 404,
  "message": "User not found with email: user@example.com",
  "errorCode": "USER_NOT_FOUND",
  "errors": {
    "email": ["Email is required"]
  },
  "timestamp": "2024-01-15T10:30:00.000Z",
  "requestId": "0HMQ8V9QKJQ1P:00000001",
  "controller": "AuthController",
  "action": "Login",
  "path": "USER_NOT_FOUND",
  "details": {
    "identifier": "user@example.com",
    "identifierType": "email"
  }
}
```

## Exception Type Handling

Both filters handle the following exception types with appropriate HTTP status codes:

| Exception Type | HTTP Status | Error Code | Description |
|----------------|-------------|------------|-------------|
| ValidationException | 400 | VALIDATION_ERROR | Input validation failed |
| UserNotFoundException | 404 | USER_NOT_FOUND | User not found |
| InvalidCredentialsException | 401 | INVALID_CREDENTIALS | Invalid login credentials |
| UserAlreadyExistsException | 409 | USER_ALREADY_EXISTS | User already exists |
| InvalidTokenException | 401 | INVALID_TOKEN | Invalid JWT token |
| AuthException | 400 | AUTH_ERROR | Generic auth error |
| Other exceptions | 500 | INTERNAL_SERVER_ERROR | Unexpected error |

## Advanced Filter Details

The AdvancedApiExceptionFilter provides additional context in the `details` field:

### ValidationException Details
```json
{
  "validationErrors": 2,
  "fieldsWithErrors": "email, password"
}
```

### UserNotFoundException Details
```json
{
  "identifier": "user@example.com",
  "identifierType": "email"
}
```

### InvalidCredentialsException Details
```json
{
  "email": "user@example.com",
  "attemptTime": "2024-01-15T10:30:00.000Z"
}
```

### UserAlreadyExistsException Details
```json
{
  "email": "user@example.com",
  "username": "john_doe"
}
```

### InvalidTokenException Details
```json
{
  "reason": "Token has expired",
  "tokenLength": 0
}
```

## Configuration

### Registration in Program.cs

```csharp
// Register API Exception Filters
builder.Services.AddScoped<ApiExceptionFilter>();
builder.Services.AddScoped<AdvancedApiExceptionFilter>();
```

### Global vs Controller-Level Usage

**Global Exception Handler (Middleware):**
- Catches all unhandled exceptions
- Provides fallback error handling
- Runs for all requests

**API Exception Filters:**
- More granular control
- Can be applied per controller or action
- Provides detailed context information
- Can be customized per endpoint

## Best Practices

### 1. Choose the Right Filter

- Use **ApiExceptionFilter** for simple, clean error responses
- Use **AdvancedApiExceptionFilter** for debugging and monitoring scenarios

### 2. Apply Filters Strategically

```csharp
// Apply to entire controller
[ApiController]
[Route("api/[controller]")]
[ApiExceptionFilter]
public class AuthController : ControllerBase
{
    // All actions inherit the filter
}

// Apply to specific actions only
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    [AdvancedApiExceptionFilter] // Detailed logging for this action
    public async Task<IActionResult> GetUser(string id) { }

    [HttpPost]
    [ApiExceptionFilter] // Basic handling for this action
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request) { }
}
```

### 3. Combine with Global Handler

The API exception filters work alongside the global exception handler:

```csharp
// Global handler catches unhandled exceptions
app.UseGlobalExceptionHandler();

// API filters provide detailed handling for specific endpoints
[ApiExceptionFilter]
public class AuthController : ControllerBase { }
```

### 4. Logging Considerations

- Both filters automatically log exceptions
- Advanced filter includes request ID for correlation
- Consider log levels and sensitive data handling

## Testing

### Unit Testing Exception Filters

```csharp
[Test]
public void ApiExceptionFilter_HandlesUserNotFoundException_Returns404()
{
    // Arrange
    var filter = new ApiExceptionFilter(mockLogger);
    var context = CreateExceptionContext(new UserNotFoundException("test@example.com"));
    
    // Act
    filter.OnException(context);
    
    // Assert
    Assert.IsTrue(context.ExceptionHandled);
    Assert.AreEqual(404, ((ObjectResult)context.Result).StatusCode);
}
```

### Integration Testing

```csharp
[Test]
public async Task Login_WithInvalidCredentials_Returns401WithDetails()
{
    // Arrange
    var client = factory.CreateClient();
    var request = new { email = "invalid@example.com", password = "wrong" };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/auth/login", request);
    var error = await response.Content.ReadFromJsonAsync<AdvancedErrorResponse>();
    
    // Assert
    Assert.AreEqual(401, response.StatusCode);
    Assert.AreEqual("INVALID_CREDENTIALS", error.ErrorCode);
    Assert.IsNotNull(error.RequestId);
    Assert.AreEqual("AuthController", error.Controller);
}
```

## Migration from Global Handler

If you want to migrate from using only the global exception handler to using API filters:

1. **Keep the global handler** as a fallback
2. **Add filters to controllers** that need detailed error handling
3. **Test thoroughly** to ensure proper exception handling
4. **Monitor logs** to verify filter behavior

## Performance Considerations

- API exception filters add minimal overhead
- Advanced filter includes more metadata but is still efficient
- Consider using basic filter for high-traffic endpoints
- Use advanced filter for debugging and monitoring scenarios 