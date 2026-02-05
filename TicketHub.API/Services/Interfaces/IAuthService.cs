using TicketHub.API.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace TicketHub.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUser(RegisterDto dto);
        Task<TokenDto?> Login(LoginDto dto);
    }
}