
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedKernel.Middlewares;

namespace SharedKernel.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
        {
            // Add Generic database context
            services.AddDbContext<TContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("SystemConnection"), 
                         sqlServerOptions =>sqlServerOptions.EnableRetryOnFailure());    
            });
            // Config Serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:ủ3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();
            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            // Use Middleware for global exception handling
            app.UseMiddleware<GlobalException>();

            // Use Middleware for listen only API Gateway
            //app.UseMiddleware<ListenOnlyApiGateway>();
            return app;
        }
    }
}
