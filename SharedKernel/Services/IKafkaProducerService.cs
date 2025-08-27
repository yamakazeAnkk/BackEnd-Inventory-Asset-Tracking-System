namespace SharedKernel.Services;

public interface IKafkaProducerService
{
    Task<bool> ProduceMessageAsync<T>(string topic, T message, string? key = null);
    Task<bool> ProduceMessageAsync(string topic, string message, string? key = null);
}
