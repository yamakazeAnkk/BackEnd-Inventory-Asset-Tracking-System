using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces;
using System.Threading.Tasks;
using AuthService.Web.Filters;
using AuthService.Web.Models;

namespace AuthService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExceptionFilter]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var result = await _authService.GetUserByIdAsync(userId);
            var response = ApiResponse<LoginResponse>.Success(result, "User retrieved successfully");
            return Ok(response);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var result = await _authService.GetUserByEmailAsync(email);
            var response = ApiResponse<LoginResponse>.Success(result, "User retrieved successfully");
            return Ok(response);
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var result = await _authService.GetUserByUsernameAsync(username);
            var response = ApiResponse<LoginResponse>.Success(result, "User retrieved successfully");
            return Ok(response);
        }

        [HttpGet("phone/{phone}")]
        public async Task<IActionResult> GetUserByPhone(string phone)
        {
            var result = await _authService.GetUserByPhoneAsync(phone);
            var response = ApiResponse<LoginResponse>.Success(result, "User retrieved successfully");
            return Ok(response);
        }
    }
}
