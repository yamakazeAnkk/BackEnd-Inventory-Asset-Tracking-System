using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Configuration;
using System.Text.Json;

namespace SharedKernel.Services;

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly KafkaOptions _kafkaOptions;

    public KafkaProducerService(IOptions<KafkaOptions> kafkaOptions, ILogger<KafkaProducerService> logger)
    {
        _kafkaOptions = kafkaOptions.Value;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            ClientId = "user-service-producer",
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000,
            LingerMs = 5,
            BatchSize = 16384
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task<bool> ProduceMessageAsync<T>(string topic, T message, string? key = null)
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            return await ProduceMessageAsync(topic, jsonMessage, key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error serializing message for topic {Topic}", topic);
            return false;
        }
    }

    public async Task<bool> ProduceMessageAsync(string topic, string message, string? key = null)
    {
        try
        {
            var kafkaMessage = new Message<string, string>
            {
                Key = key ?? Guid.NewGuid().ToString(),
                Value = message
            };

            var result = await _producer.ProduceAsync(topic, kafkaMessage);
            
            _logger.LogInformation("Message produced to topic {Topic} at partition {Partition} with offset {Offset}", 
                result.Topic, result.Partition, result.Offset);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing message to topic {Topic}", topic);
            return false;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
