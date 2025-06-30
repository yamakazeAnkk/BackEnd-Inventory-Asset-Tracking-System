using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AuthService.Application.Dtos.Auth;
using AuthService.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.ExceptionHandling
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = new ApiResponse<object>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = null
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case ValidationException validationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new ErrorResponse
                    {
                        StatusCode = response.StatusCode,
                        Message = validationEx.Message,
                        ErrorCode = validationEx.ErrorCode,
                        Errors = validationEx.Errors
                    };
                    break;

                case UserNotFoundException userNotFoundEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new ErrorResponse
                    {
                        StatusCode = response.StatusCode,
                        Message = userNotFoundEx.Message,
                        ErrorCode = userNotFoundEx.ErrorCode
                    };
                    break;

                case InvalidCredentialsException invalidCredEx:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse = new ErrorResponse
                    {
                        StatusCode = response.StatusCode,
                        Message = invalidCredEx.Message,
                        ErrorCode = invalidCredEx.ErrorCode
                    };
                    break;

                case UserAlreadyExistsException userExistsEx:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse = new ErrorResponse
                    {
                        StatusCode = response.StatusCode,
                        Message = userExistsEx.Message,
                        ErrorCode = userExistsEx.ErrorCode
                    };
                    break;

                case InvalidTokenException invalidTokenEx:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse = new ErrorResponse
                    {
                        StatusCode = response.StatusCode,
                        Message = invalidTokenEx.Message,
                        ErrorCode = invalidTokenEx.ErrorCode
                    };
                    break;

                case AuthException authEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new ErrorResponse
                    {
                        StatusCode = response.StatusCode,
                        Message = authEx.Message,
                        ErrorCode = authEx.ErrorCode
                    };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new ErrorResponse
                    {
                        StatusCode = response.StatusCode,
                        Message = "An unexpected error occurred.",
                        ErrorCode = "INTERNAL_SERVER_ERROR"
                    };
                    break;
            }

            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(result);
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 