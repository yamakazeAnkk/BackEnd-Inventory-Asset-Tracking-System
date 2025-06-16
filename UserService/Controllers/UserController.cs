using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private static List<string> _users = new() { "User 1", "User 2" };
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_users);
        }
    }
}