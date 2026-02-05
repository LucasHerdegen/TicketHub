using TicketHub.API.DTOs;
using Microsoft.AspNetCore.Identity;

namespace TicketHub.API.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUser(RegisterDto dto);
        Task<TokenDto?> Login(LoginDto dto);
    }
}