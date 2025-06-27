using Microsoft.AspNetCore.Mvc;
using AuthService.Web.Models;

namespace AuthService.Web.Controllers
{
    [ApiController]
    [ApiExceptionFilter]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult SuccessResponse<T>(T data, string message = "Operation completed successfully")
        {
            var response = ApiResponse<T>.Success(data, message);
            return Ok(response);
        }

        protected IActionResult SuccessResponse(string message = "Operation completed successfully")
        {
            var response = ApiResponse.Success(message);
            return Ok(response);
        }

        protected IActionResult FailureResponse<T>(string message, T data = default)
        {
            var response = ApiResponse<T>.Failure(message, data);
            return BadRequest(response);
        }

        protected IActionResult FailureResponse(string message)
        {
            var response = ApiResponse.Failure(message);
            return BadRequest(response);
        }

        protected IActionResult NotFoundResponse(string message = "Resource not found")
        {
            var response = ApiResponse.Failure(message);
            return NotFound(response);
        }

        protected IActionResult UnauthorizedResponse(string message = "Unauthorized access")
        {
            var response = ApiResponse.Failure(message);
            return Unauthorized(response);
        }
    }
} 