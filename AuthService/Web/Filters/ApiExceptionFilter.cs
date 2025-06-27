using System;
using System.Net;
using System.Text.Json;
using AuthService.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using AuthService.Web.Filters;
namespace AuthService.Web.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;
        

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An exception occurred in {Controller}.{Action}: {Message}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"],
                context.Exception.Message);

            var result = new ObjectResult(CreateErrorResponse(context.Exception))
            {
                StatusCode = GetStatusCode(context.Exception)
            };

            context.Result = result;
            context.ExceptionHandled = true;
        }

        private ErrorResponse CreateErrorResponse(Exception exception)
        {
            return exception switch
            {
                ValidationException validationEx => new ErrorResponse
                {
                    StatusCode = GetStatusCode(validationEx),
                    Message = validationEx.Message,
                    ErrorCode = validationEx.ErrorCode,
                    Errors = validationEx.Errors,
                    Timestamp = DateTime.UtcNow
                },

                UserNotFoundException userNotFoundEx => new ErrorResponse
                {
                    StatusCode = GetStatusCode(userNotFoundEx),
                    Message = userNotFoundEx.Message,
                    ErrorCode = userNotFoundEx.ErrorCode,
                    Timestamp = DateTime.UtcNow
                },

                InvalidCredentialsException invalidCredEx => new ErrorResponse
                {
                    StatusCode = GetStatusCode(invalidCredEx),
                    Message = invalidCredEx.Message,
                    ErrorCode = invalidCredEx.ErrorCode,
                    Timestamp = DateTime.UtcNow
                },

                UserAlreadyExistsException userExistsEx => new ErrorResponse
                {
                    StatusCode = GetStatusCode(userExistsEx),
                    Message = userExistsEx.Message,
                    ErrorCode = userExistsEx.ErrorCode,
                    Timestamp = DateTime.UtcNow
                },

                InvalidTokenException invalidTokenEx => new ErrorResponse
                {
                    StatusCode = GetStatusCode(invalidTokenEx),
                    Message = invalidTokenEx.Message,
                    ErrorCode = invalidTokenEx.ErrorCode,
                    Timestamp = DateTime.UtcNow
                },

                AuthException authEx => new ErrorResponse
                {
                    StatusCode = GetStatusCode(authEx),
                    Message = authEx.Message,
                    ErrorCode = authEx.ErrorCode,
                    Timestamp = DateTime.UtcNow
                },

                _ => new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "An unexpected error occurred.",
                    ErrorCode = "INTERNAL_SERVER_ERROR",
                    Timestamp = DateTime.UtcNow
                }
            };
        }

        private int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ValidationException => (int)HttpStatusCode.BadRequest,
                UserNotFoundException => (int)HttpStatusCode.NotFound,
                InvalidCredentialsException => (int)HttpStatusCode.Unauthorized,
                UserAlreadyExistsException => (int)HttpStatusCode.Conflict,
                InvalidTokenException => (int)HttpStatusCode.Unauthorized,
                AuthException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };
        }
    }

} 