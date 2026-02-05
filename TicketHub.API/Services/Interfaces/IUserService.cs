using TicketHub.API.DTOs.User;

namespace TicketHub.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>?> GetUsers();

        Task<UserDto?> GetUserByEmail(string email);
    }
}