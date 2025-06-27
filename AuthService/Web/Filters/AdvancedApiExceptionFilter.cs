using System;
using System.Net;
using System.Text.Json;
using AuthService.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using AuthService.Web.Filters;

namespace AuthService.Web.Filters
{
    public class AdvancedApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<AdvancedApiExceptionFilter> _logger;

        public AdvancedApiExceptionFilter(ILogger<AdvancedApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "Unknown";
            var actionName = context.RouteData.Values["action"]?.ToString() ?? "Unknown";
            var requestId = context.HttpContext.TraceIdentifier;

            _logger.LogError(context.Exception, 
                "Exception in {Controller}.{Action} (RequestId: {RequestId}): {Message}",
                controllerName, actionName, requestId, context.Exception.Message);

            var errorResponse = CreateAdvancedErrorResponse(context.Exception, requestId, controllerName, actionName);
            
            var result = new ObjectResult(errorResponse)
            {
                StatusCode = GetStatusCode(context.Exception)
            };

            context.Result = result;
            context.ExceptionHandled = true;
        }

        private AdvancedErrorResponse CreateAdvancedErrorResponse(Exception exception, string requestId, string controllerName, string actionName)
        {
            var baseResponse = new AdvancedErrorResponse
            {
                RequestId = requestId,
                Controller = controllerName,
                Action = actionName,
                Timestamp = DateTime.UtcNow,
                Path = exception switch
                {
                    ValidationException => "VALIDATION_ERROR",
                    UserNotFoundException => "USER_NOT_FOUND",
                    InvalidCredentialsException => "AUTHENTICATION_ERROR",
                    UserAlreadyExistsException => "CONFLICT_ERROR",
                    InvalidTokenException => "TOKEN_ERROR",
                    AuthException => "AUTH_ERROR",
                    _ => "INTERNAL_ERROR"
                }
            };

            return exception switch
            {
                ValidationException validationEx => new AdvancedErrorResponse
                {
                    StatusCode = GetStatusCode(validationEx),
                    Message = validationEx.Message,
                    ErrorCode = validationEx.ErrorCode,
                    Errors = validationEx.Errors,
                    RequestId = requestId,
                    Controller = controllerName,
                    Action = actionName,
                    Timestamp = DateTime.UtcNow,
                    Path = "VALIDATION_ERROR",
                    Details = new Dictionary<string, object>
                    {
                        ["ValidationErrors"] = validationEx.Errors.Count,
                        ["FieldsWithErrors"] = string.Join(", ", validationEx.Errors.Keys)
                    }
                },

                UserNotFoundException userNotFoundEx => new AdvancedErrorResponse
                {
                    StatusCode = GetStatusCode(userNotFoundEx),
                    Message = userNotFoundEx.Message,
                    ErrorCode = userNotFoundEx.ErrorCode,
                    RequestId = requestId,
                    Controller = controllerName,
                    Action = actionName,
                    Timestamp = DateTime.UtcNow,
                    Path = "USER_NOT_FOUND",
                    Details = new Dictionary<string, object>
                    {
                        ["Identifier"] = userNotFoundEx.Identifier,
                        ["IdentifierType"] = userNotFoundEx.IdentifierType
                    }
                },

                InvalidCredentialsException invalidCredEx => new AdvancedErrorResponse
                {
                    StatusCode = GetStatusCode(invalidCredEx),
                    Message = invalidCredEx.Message,
                    ErrorCode = invalidCredEx.ErrorCode,
                    RequestId = requestId,
                    Controller = controllerName,
                    Action = actionName,
                    Timestamp = DateTime.UtcNow,
                    Path = "AUTHENTICATION_ERROR",
                    Details = new Dictionary<string, object>
                    {
                        ["Email"] = invalidCredEx.Email,
                        ["AttemptTime"] = DateTime.UtcNow
                    }
                },

                UserAlreadyExistsException userExistsEx => new AdvancedErrorResponse
                {
                    StatusCode = GetStatusCode(userExistsEx),
                    Message = userExistsEx.Message,
                    ErrorCode = userExistsEx.ErrorCode,
                    RequestId = requestId,
                    Controller = controllerName,
                    Action = actionName,
                    Timestamp = DateTime.UtcNow,
                    Path = "CONFLICT_ERROR",
                    Details = new Dictionary<string, object>
                    {
                        ["Email"] = userExistsEx.Email,
                        ["Username"] = userExistsEx.Username
                    }
                },

                InvalidTokenException invalidTokenEx => new AdvancedErrorResponse
                {
                    StatusCode = GetStatusCode(invalidTokenEx),
                    Message = invalidTokenEx.Message,
                    ErrorCode = invalidTokenEx.ErrorCode,
                    RequestId = requestId,
                    Controller = controllerName,
                    Action = actionName,
                    Timestamp = DateTime.UtcNow,
                    Path = "TOKEN_ERROR",
                    Details = new Dictionary<string, object>
                    {
                        ["Reason"] = invalidTokenEx.Reason,
                        ["TokenLength"] = invalidTokenEx.Token?.Length ?? 0
                    }
                },

                AuthException authEx => new AdvancedErrorResponse
                {
                    StatusCode = GetStatusCode(authEx),
                    Message = authEx.Message,
                    ErrorCode = authEx.ErrorCode,
                    RequestId = requestId,
                    Controller = controllerName,
                    Action = actionName,
                    Timestamp = DateTime.UtcNow,
                    Path = "AUTH_ERROR"
                },

                _ => new AdvancedErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "An unexpected error occurred.",
                    ErrorCode = "INTERNAL_SERVER_ERROR",
                    RequestId = requestId,
                    Controller = controllerName,
                    Action = actionName,
                    Timestamp = DateTime.UtcNow,
                    Path = "INTERNAL_ERROR",
                    Details = new Dictionary<string, object>
                    {
                        ["ExceptionType"] = exception.GetType().Name,
                        ["StackTrace"] = exception.StackTrace?.Split('\n')[0] // First line only for security
                    }
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