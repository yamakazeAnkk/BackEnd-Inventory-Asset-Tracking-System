using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("AuthService is running!");
    }
} 