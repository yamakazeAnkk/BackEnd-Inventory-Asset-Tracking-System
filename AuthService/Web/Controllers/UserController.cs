using Microsoft.AspNetCore.Mvc;
using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces;
using System.Threading.Tasks;
using AuthService.Web.Filters;
using AuthService.Application.Dtos.Auth;
using AutoMapper;
using AuthService.Web.Filters;

namespace AuthService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExceptionFilter]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly IMapper _mapper;


        
        public UserController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var result = await _authService.GetUserByIdAsync(userId);
            
            return Ok(result);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var result = await _authService.GetUserByEmailAsync(email);
            return Ok(result);
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var result = await _authService.GetUserByUsernameAsync(username);
            return Ok(result);
        }

        [HttpGet("phone/{phone}")]
        public async Task<IActionResult> GetUserByPhone(string phone)
        {
            var result = await _authService.GetUserByPhoneAsync(phone);
            return Ok(result);
        }
    }
}
