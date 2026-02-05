using TicketHub.API.DTOs.User;
using TicketHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace TicketHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [OutputCache]
        public async Task<ActionResult<IEnumerable<UserDto>?>> GetUsers()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{email}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);

            if (user == null)
                return NotFound($"User {email} not found");

            return Ok(user);
        }
    }
}