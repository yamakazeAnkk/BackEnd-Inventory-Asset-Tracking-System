using Microsoft.AspNetCore.Mvc;
using SharedKernel.Services;
using SharedKernel.Messages;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KafkaTestController : ControllerBase
{
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ILogger<KafkaTestController> _logger;

    public KafkaTestController(
        IKafkaProducerService kafkaProducer,
        ILogger<KafkaTestController> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    [HttpPost("publish-test-message")]
    public async Task<IActionResult> PublishTestMessage([FromBody] string message)
    {
        try
        {
            var success = await _kafkaProducer.ProduceMessageAsync("test-topic", message);
            
            if (success)
            {
                _logger.LogInformation("Test message published successfully: {Message}", message);
                return Ok(new { success = true, message = "Message published successfully" });
            }
            else
            {
                _logger.LogError("Failed to publish test message: {Message}", message);
                return BadRequest(new { success = false, message = "Failed to publish message" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing test message: {Message}", message);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new 
        { 
            service = "UserService", 
            kafka = "Connected",
            timestamp = DateTime.UtcNow,
            message = "UserService is running and listening to Kafka messages"
        });
    }
}
