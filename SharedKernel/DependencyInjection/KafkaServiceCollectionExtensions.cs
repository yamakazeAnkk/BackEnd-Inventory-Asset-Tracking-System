using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Configuration;
using SharedKernel.Services;

namespace SharedKernel.DependencyInjection;

public static class KafkaServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Kafka options
        services.Configure<KafkaOptions>(configuration.GetSection(KafkaOptions.SectionName));

        // Register Kafka producer and consumer services
        services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
        services.AddSingleton<IKafkaConsumerService, KafkaConsumerService>();

        return services;
    }
}
