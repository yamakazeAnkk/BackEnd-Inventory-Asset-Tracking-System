using SharedKernel.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace SharedKernel.Middlewares
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "Sorry, internal server error occurred. Please try again later.";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                // Exception is Too Many Request - status code 429
                if(context.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
                {
                    message = "Too many requests. Please try again later.";
                    statusCode = (int)HttpStatusCode.TooManyRequests;
                    title = "Warning";
                    await ModifyHeader(context, message, statusCode, title);
                }
                // Exception is UnAuthorized - status code 401
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    message = "Unauthorized access. Please login to continue.";
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    title = "Alert";
                    await ModifyHeader(context, message, statusCode, title);
                }

                // Exception is Forbidden - status code 403
                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    message = "Forbidden access. You do not have permission to access this resource.";
                    statusCode = (int)HttpStatusCode.Forbidden;
                    title = "Out of Access";
                    await ModifyHeader(context, message, statusCode, title);
                }
            }
            catch (Exception ex)
            {
                // Log original Exceptions /File, Debugger, Console
                LogException.LogExceptions(ex);

                // Check if the exception is timeout - status code 408
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    message = "Request timeout. Please try again later.";
                    statusCode = (int)HttpStatusCode.RequestTimeout;
                    title = "Out of time";
                }

                // If none of the above conditions are met, do default.
                await ModifyHeader(context, message, statusCode, title);
            }
        }

        private async Task ModifyHeader(HttpContext context, string message, int statusCode, string title)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title,
            }), CancellationToken.None);
            return;
        }
    }
}
