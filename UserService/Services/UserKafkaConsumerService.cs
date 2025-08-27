using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedKernel.Services;
using SharedKernel.Messages;
using System.Text.Json;

namespace UserService.Services;

public class UserKafkaConsumerService : BackgroundService
{
    private readonly IKafkaConsumerService _kafkaConsumer;
    private readonly ILogger<UserKafkaConsumerService> _logger;

    public UserKafkaConsumerService(
        IKafkaConsumerService kafkaConsumer,
        ILogger<UserKafkaConsumerService> logger)
    {
        _kafkaConsumer = kafkaConsumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting User Kafka Consumer Service");

        // Consume user-related messages from AuthService
        await _kafkaConsumer.StartConsumingAsync("user-events", HandleUserMessage, stoppingToken);
    }

    private async Task HandleUserMessage(string message)
    {
        try
        {
            _logger.LogInformation("Received message: {Message}", message);

            // Try to deserialize as different message types
            if (TryDeserializeMessage<UserCreatedMessage>(message, out var userCreated))
            {
                await HandleUserCreated(userCreated);
            }
            else if (TryDeserializeMessage<UserUpdatedMessage>(message, out var userUpdated))
            {
                await HandleUserUpdated(userUpdated);
            }
            else if (TryDeserializeMessage<UserDeletedMessage>(message, out var userDeleted))
            {
                await HandleUserDeleted(userDeleted);
            }
            else if (TryDeserializeMessage<UserLoginMessage>(message, out var userLogin))
            {
                await HandleUserLogin(userLogin);
            }
            else if (TryDeserializeMessage<UserLogoutMessage>(message, out var userLogout))
            {
                await HandleUserLogout(userLogout);
            }
            else
            {
                _logger.LogWarning("Unknown message format: {Message}", message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling user message: {Message}", message);
        }
    }

    private bool TryDeserializeMessage<T>(string message, out T? result) where T : class
    {
        try
        {
            result = JsonSerializer.Deserialize<T>(message);
            return result != null;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    private Task HandleUserCreated(UserCreatedMessage message)
    {
        _logger.LogInformation("User created: {UserId} - {Email}", message.UserId, message.Email);
        // TODO: Implement user creation logic in UserService
        return Task.CompletedTask;
    }

    private Task HandleUserUpdated(UserUpdatedMessage message)
    {
        _logger.LogInformation("User updated: {UserId} - {Email}", message.UserId, message.Email);
        // TODO: Implement user update logic in UserService
        return Task.CompletedTask;
    }

    private Task HandleUserDeleted(UserDeletedMessage message)
    {
        _logger.LogInformation("User deleted: {UserId}", message.UserId);
        // TODO: Implement user deletion logic in UserService
        return Task.CompletedTask;
    }

    private Task HandleUserLogin(UserLoginMessage message)
    {
        _logger.LogInformation("User login: {UserId} - {Email} from {IpAddress}", 
            message.UserId, message.Email, message.IpAddress);
        // TODO: Implement user login tracking logic in UserService
        return Task.CompletedTask;
    }

    private Task HandleUserLogout(UserLogoutMessage message)
    {
        _logger.LogInformation("User logout: {UserId}", message.UserId);
        // TODO: Implement user logout tracking logic in UserService
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping User Kafka Consumer Service");
        await _kafkaConsumer.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }
}
