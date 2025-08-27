using Microsoft.AspNetCore.Mvc;
using AuthService.Infrastructure.Services;
using SharedKernel.Messages;

namespace AuthService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KafkaTestController : ControllerBase
{
    private readonly IUserEventPublisherService _userEventPublisher;
    private readonly ILogger<KafkaTestController> _logger;

    public KafkaTestController(
        IUserEventPublisherService userEventPublisher,
        ILogger<KafkaTestController> logger)
    {
        _userEventPublisher = userEventPublisher;
        _logger = logger;
    }

    [HttpPost("publish-user-created")]
    public async Task<IActionResult> PublishUserCreated([FromBody] UserCreatedMessage message)
    {
        try
        {
            await _userEventPublisher.PublishUserCreatedAsync(message.UserId, message.Email, message.Username);
            return Ok(new { success = true, message = "User created event published successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing user created event");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpPost("publish-user-login")]
    public async Task<IActionResult> PublishUserLogin([FromBody] UserLoginMessage message)
    {
        try
        {
            await _userEventPublisher.PublishUserLoginAsync(message.UserId, message.Email, message.IpAddress);
            return Ok(new { success = true, message = "User login event published successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing user login event");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new 
        { 
            service = "AuthService", 
            kafka = "Connected",
            timestamp = DateTime.UtcNow,
            message = "AuthService is running and can publish user events to Kafka"
        });
    }
}
