namespace SharedKernel.Services;

public interface IKafkaConsumerService
{
    Task StartConsumingAsync(string topic, Func<string, Task> messageHandler, CancellationToken cancellationToken = default);
    Task StopConsumingAsync();
}
