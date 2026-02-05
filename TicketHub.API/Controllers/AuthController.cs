using TicketHub.API.DTOs;
using TicketHub.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TicketHub.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _userService;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;

        public AuthController(IValidator<RegisterDto> registerValidator,
            IAuthService userService,
            IValidator<LoginDto> loginValidator)
        {
            _registerValidator = registerValidator;
            _userService = userService;
            _loginValidator = loginValidator;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var validation = _registerValidator.Validate(dto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var result = await _userService.RegisterUser(dto);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Successfully registered");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var validation = _loginValidator.Validate(dto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var tokenResult = await _userService.Login(dto);

            if (tokenResult == null)
                return Unauthorized("Something went wrong! Check the credentials");

            return Ok(tokenResult);
        }
    }
}