using AssetService.Infrastructure.Data;
using AssetService.Domain.Interfaces;
using AssetService.Infrastructure.Repositories;
using SharedKernel.DependencyInjection;

namespace AssetService.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {

        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // add db connection
            // add authen scheme
            SharedServiceContainer.AddSharedServices<AssetDbContext>(services, config, config["MySerilog:FileName"] ?? "AssetApiLog");

            // create DI 
            services.AddScoped<IAsset, AssetRepository>();
            services.AddScoped<ICategory, CategoryRepository>();
            services.AddScoped<ISupplier, SupplierRepository>();
            services.AddScoped<IBrand, BrandRepository>();

            return services;

        }

        public static IApplicationBuilder UseInfrastructureService(this IApplicationBuilder app)
        {
            // add middleware
            // use global exception handler
            // Listen only API Gateway

            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
