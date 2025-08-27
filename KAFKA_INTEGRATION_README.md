# Kafka Integration between UserService and AuthService

This document describes the Kafka integration setup between the UserService and AuthService in the Inventory Asset System.

## 🏗️ Architecture Overview

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   AuthService   │───▶│     Kafka      │───▶│  UserService    │
│                 │    │  (localhost:9092)│    │                 │
│ Publishes User  │    │                 │    │ Consumes User   │
│ Events          │    │                 │    │ Events          │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 📋 Prerequisites

- Kafka running locally (via Homebrew)
- .NET 8.0 SDK
- Both services built successfully

## 🚀 Quick Start

### 1. Start Kafka (if not running)
```bash
brew services start kafka
```

### 2. Verify Kafka is running
```bash
brew services list | grep kafka
```

### 3. Check existing topics
```bash
kafka-topics --bootstrap-server localhost:9092 --list
```

### 4. Run the integration test
```bash
./test-kafka-integration.sh
```

## 🔧 Manual Testing

### Start UserService
```bash
cd UserService
dotnet run
```

### Start AuthService (in another terminal)
```bash
cd AuthService
dotnet run
```

### Test Endpoints

#### UserService Kafka Test Endpoints
- **GET** `/api/kafkatest/status` - Check service status
- **POST** `/api/kafkatest/publish-test-message` - Publish test message

#### AuthService Kafka Test Endpoints
- **GET** `/api/kafkatest/status` - Check service status
- **POST** `/api/kafkatest/publish-user-created` - Publish user created event
- **POST** `/api/kafkatest/publish-user-login` - Publish user login event

## 📨 Message Types

### User Events (user-events topic)

#### UserCreatedMessage
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "email": "user@example.com",
  "username": "username",
  "createdAt": "2025-01-27T14:00:00Z"
}
```

#### UserUpdatedMessage
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "email": "user@example.com",
  "username": "username",
  "updatedAt": "2025-01-27T14:00:00Z"
}
```

#### UserDeletedMessage
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "deletedAt": "2025-01-27T14:00:00Z"
}
```

#### UserLoginMessage
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "email": "user@example.com",
  "loginAt": "2025-01-27T14:00:00Z",
  "ipAddress": "192.168.1.100"
}
```

#### UserLogoutMessage
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "logoutAt": "2025-01-27T14:00:00Z"
}
```

## ⚙️ Configuration

### Kafka Configuration (appsettings.json)

Both services include Kafka configuration:

```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "service-specific-group",
    "AutoOffsetReset": 0,
    "EnableAutoCommit": true,
    "AutoCommitIntervalMs": 1000,
    "SessionTimeoutMs": 30000,
    "HeartbeatIntervalMs": 3000
  }
}
```

### Service-Specific Group IDs
- **UserService**: `user-service-group`
- **AuthService**: `auth-service-group`

## 🏗️ Implementation Details

### SharedKernel Components

#### KafkaOptions
Configuration class for Kafka settings with sensible defaults.

#### IKafkaProducerService / KafkaProducerService
- Thread-safe Kafka producer
- Automatic JSON serialization
- Configurable retry and batching settings
- Proper disposal of resources

#### IKafkaConsumerService / KafkaConsumerService
- Kafka consumer with automatic offset management
- Background message processing
- Graceful shutdown handling
- Error handling and logging

### Service Integration

#### UserService
- **UserKafkaConsumerService**: Background service that consumes user events
- Listens to `user-events` topic
- Processes different message types automatically
- Logs all received messages

#### AuthService
- **UserEventPublisherService**: Publishes user events to Kafka
- Integrates with existing user management operations
- Publishes events for user lifecycle operations

## 📊 Monitoring and Debugging

### Check Consumer Groups
```bash
kafka-consumer-groups --bootstrap-server localhost:9092 --list
kafka-consumer-groups --bootstrap-server localhost:9092 --describe --group user-service-group
```

### Monitor Topics
```bash
# Monitor user-events topic
kafka-console-consumer --bootstrap-server localhost:9092 --topic user-events --from-beginning

# Monitor test-topic
kafka-console-consumer --bootstrap-server localhost:9092 --topic test-topic --from-beginning
```

### View Service Logs
Both services log Kafka operations:
- Message production success/failure
- Message consumption and processing
- Error conditions and retry attempts

## 🔒 Security Considerations

- Current setup uses localhost Kafka (development only)
- No authentication/authorization configured
- Consider SASL/SSL for production environments
- Implement proper error handling and dead letter queues

## 🚨 Troubleshooting

### Common Issues

1. **Kafka not running**
   ```bash
   brew services start kafka
   ```

2. **Port conflicts**
   - UserService: 5000
   - AuthService: 5001
   - Kafka: 9092

3. **Build errors**
   - Ensure SharedKernel builds first
   - Check package version compatibility
   - Verify project references

4. **Message not received**
   - Check consumer group configuration
   - Verify topic exists and has messages
   - Check service logs for errors

### Debug Commands

```bash
# Check Kafka status
brew services list | grep kafka

# List topics
kafka-topics --bootstrap-server localhost:9092 --list

# Check consumer groups
kafka-consumer-groups --bootstrap-server localhost:9092 --list

# Monitor specific topic
kafka-console-consumer --bootstrap-server localhost:9092 --topic user-events --from-beginning
```

## 🔮 Future Enhancements

1. **Message Schema Validation**
   - Implement Avro or JSON Schema
   - Add message versioning

2. **Dead Letter Queue**
   - Handle failed message processing
   - Implement retry mechanisms

3. **Message Persistence**
   - Store processed messages
   - Implement audit trails

4. **Health Checks**
   - Kafka connectivity monitoring
   - Consumer lag monitoring

5. **Metrics and Alerting**
   - Message throughput monitoring
   - Error rate alerting

## 📚 Additional Resources

- [Confluent Kafka .NET Client](https://github.com/confluentinc/confluent-kafka-dotnet)
- [Apache Kafka Documentation](https://kafka.apache.org/documentation/)
- [.NET Background Services](https://docs.microsoft.com/en-us/dotnet/core/extensions/workers)

## 🤝 Contributing

When adding new message types or modifying the Kafka integration:

1. Update message models in `SharedKernel/Messages/`
2. Add corresponding handlers in consuming services
3. Update configuration if needed
4. Add tests for new functionality
5. Update this documentation

---

**Note**: This integration is designed for development and testing. Production deployments should include proper security, monitoring, and error handling configurations.
