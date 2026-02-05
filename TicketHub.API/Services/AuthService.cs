using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using TicketHub.API.DTOs.User;
using TicketHub.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace TicketHub.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterUser(RegisterDto dto)
        {
            var newUser = _mapper.Map<ApplicationUser>(dto);

            var result = await _userManager.CreateAsync(newUser, dto.Password!);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(newUser, "User");

            return result;
        }

        public async Task<TokenDto?> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email!);

            if (user == null)
                return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password!, false);

            if (!result.Succeeded)
                return null;

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, dto.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]!));

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token).ToString();

            return new TokenDto
            {
                Token = tokenString,
                ValidTo = token.ValidTo
            };
        }
    }
}