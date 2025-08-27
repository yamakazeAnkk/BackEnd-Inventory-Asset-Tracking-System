using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Configuration;

namespace SharedKernel.Services;

public class KafkaConsumerService : IKafkaConsumerService, IDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly KafkaOptions _kafkaOptions;
    private bool _isConsuming = false;

    public KafkaConsumerService(IOptions<KafkaOptions> kafkaOptions, ILogger<KafkaConsumerService> logger)
    {
        _kafkaOptions = kafkaOptions.Value;
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _kafkaOptions.GroupId,
            AutoOffsetReset = (AutoOffsetReset)_kafkaOptions.AutoOffsetReset,
            EnableAutoCommit = _kafkaOptions.EnableAutoCommit,
            AutoCommitIntervalMs = _kafkaOptions.AutoCommitIntervalMs,
            SessionTimeoutMs = _kafkaOptions.SessionTimeoutMs,
            HeartbeatIntervalMs = _kafkaOptions.HeartbeatIntervalMs,
            EnablePartitionEof = true
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    public async Task StartConsumingAsync(string topic, Func<string, Task> messageHandler, CancellationToken cancellationToken = default)
    {
        if (_isConsuming)
        {
            _logger.LogWarning("Consumer is already running for topic {Topic}", topic);
            return;
        }

        _isConsuming = true;
        _consumer.Subscribe(topic);

        _logger.LogInformation("Started consuming from topic {Topic}", topic);

        try
        {
            while (_isConsuming && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    
                    if (consumeResult.IsPartitionEOF)
                    {
                        _logger.LogDebug("Reached end of partition {Partition} for topic {Topic}", 
                            consumeResult.Partition, consumeResult.Topic);
                        continue;
                    }

                    _logger.LogDebug("Received message from topic {Topic} partition {Partition} offset {Offset}", 
                        consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);

                    await messageHandler(consumeResult.Message.Value);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Consuming cancelled for topic {Topic}", topic);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming message from topic {Topic}", topic);
                }
            }
        }
        finally
        {
            _isConsuming = false;
            _consumer.Close();
        }
    }

    public Task StopConsumingAsync()
    {
        _isConsuming = false;
        _consumer?.Close();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _consumer?.Dispose();
    }
}
