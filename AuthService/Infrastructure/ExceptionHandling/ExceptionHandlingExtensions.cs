using Microsoft.AspNetCore.Builder;

namespace AuthService.Infrastructure.ExceptionHandling
{
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandler>();
        }
    }
} 