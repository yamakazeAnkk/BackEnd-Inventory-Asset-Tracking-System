using Microsoft.Extensions.Logging;
using SharedKernel.Services;
using SharedKernel.Messages;

namespace AuthService.Infrastructure.Services;

public interface IUserEventPublisherService
{
    Task PublishUserCreatedAsync(Guid userId, string email, string username);
    Task PublishUserUpdatedAsync(Guid userId, string email, string username);
    Task PublishUserDeletedAsync(Guid userId);
    Task PublishUserLoginAsync(Guid userId, string email, string ipAddress);
    Task PublishUserLogoutAsync(Guid userId);
}

public class UserEventPublisherService : IUserEventPublisherService
{
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ILogger<UserEventPublisherService> _logger;

    public UserEventPublisherService(
        IKafkaProducerService kafkaProducer,
        ILogger<UserEventPublisherService> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public async Task PublishUserCreatedAsync(Guid userId, string email, string username)
    {
        var message = new UserCreatedMessage
        {
            UserId = userId,
            Email = email,
            Username = username,
            CreatedAt = DateTime.UtcNow
        };

        var success = await _kafkaProducer.ProduceMessageAsync("user-events", message, userId.ToString());
        
        if (success)
        {
            _logger.LogInformation("Published user created event for user {UserId}", userId);
        }
        else
        {
            _logger.LogError("Failed to publish user created event for user {UserId}", userId);
        }
    }

    public async Task PublishUserUpdatedAsync(Guid userId, string email, string username)
    {
        var message = new UserUpdatedMessage
        {
            UserId = userId,
            Email = email,
            Username = username,
            UpdatedAt = DateTime.UtcNow
        };

        var success = await _kafkaProducer.ProduceMessageAsync("user-events", message, userId.ToString());
        
        if (success)
        {
            _logger.LogInformation("Published user updated event for user {UserId}", userId);
        }
        else
        {
            _logger.LogError("Failed to publish user updated event for user {UserId}", userId);
        }
    }

    public async Task PublishUserDeletedAsync(Guid userId)
    {
        var message = new UserDeletedMessage
        {
            UserId = userId,
            DeletedAt = DateTime.UtcNow
        };

        var success = await _kafkaProducer.ProduceMessageAsync("user-events", message, userId.ToString());
        
        if (success)
        {
            _logger.LogInformation("Published user deleted event for user {UserId}", userId);
        }
        else
        {
            _logger.LogError("Failed to publish user deleted event for user {UserId}", userId);
        }
    }

    public async Task PublishUserLoginAsync(Guid userId, string email, string ipAddress)
    {
        var message = new UserLoginMessage
        {
            UserId = userId,
            Email = email,
            LoginAt = DateTime.UtcNow,
            IpAddress = ipAddress
        };

        var success = await _kafkaProducer.ProduceMessageAsync("user-events", message, userId.ToString());
        
        if (success)
        {
            _logger.LogInformation("Published user login event for user {UserId}", userId);
        }
        else
        {
            _logger.LogError("Failed to publish user login event for user {UserId}", userId);
        }
    }

    public async Task PublishUserLogoutAsync(Guid userId)
    {
        var message = new UserLogoutMessage
        {
            UserId = userId,
            LogoutAt = DateTime.UtcNow
        };

        var success = await _kafkaProducer.ProduceMessageAsync("user-events", message, userId.ToString());
        
        if (success)
        {
            _logger.LogInformation("Published user logout event for user {UserId}", userId);
        }
        else
        {
            _logger.LogError("Failed to publish user logout event for user {UserId}", userId);
        }
    }
}
