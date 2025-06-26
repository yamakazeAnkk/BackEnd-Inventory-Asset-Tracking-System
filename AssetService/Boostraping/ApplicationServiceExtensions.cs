using AssetService.Interface;
using AssetService.Repository;

namespace AssetService.Boostraping
{
    public static class  ApplicationServiceExtensions
    {
        public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
        {
            // Add application services here
            // For example, you can add a DbContext, repositories, etc.
            // builder.Services.AddDbContext<AssetDbContext>(options => ...);
            // builder.Services.AddScoped<IAssetRepository, AssetRepository>();
           builder.Services.AddScoped<IAsset, AssetRepository>();

            return builder;
        }
    }
}
