namespace SharedKernel.Configuration;

public class KafkaOptions
{
    public const string SectionName = "Kafka";
    
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string GroupId { get; set; } = "default-group";
    public int AutoOffsetReset { get; set; } = 0; // 0 = earliest, 1 = latest
    public bool EnableAutoCommit { get; set; } = true;
    public int AutoCommitIntervalMs { get; set; } = 1000;
    public int SessionTimeoutMs { get; set; } = 30000;
    public int HeartbeatIntervalMs { get; set; } = 3000;
}
