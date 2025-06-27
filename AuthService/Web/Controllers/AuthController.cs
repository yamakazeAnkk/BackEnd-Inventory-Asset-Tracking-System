using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Dtos.Auth;
using AuthService.Application.Interfaces;
using System.Threading.Tasks;
using AuthService.Web.Filters;

namespace AuthService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExceptionFilter]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var response = await _authService.GetUserByEmailAsync(email);
            return Ok(response);
        }

        [HttpGet("user/username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var response = await _authService.GetUserByUsernameAsync(username);
            return Ok(response);
        }
    }
}
