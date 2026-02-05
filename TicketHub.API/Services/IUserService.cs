using TicketHub.API.DTOs;

namespace TicketHub.API.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>?> GetUsers();

        Task<UserDto?> GetUserByEmail(string email);
    }
}