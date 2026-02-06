using TicketHub.API.DTOs.Ticket;
using TicketHub.API.Pagination;

namespace TicketHub.API.Services.Interfaces
{
    public interface ITicketService
    {
        Task<PagedList<TicketDto>> GetTickets(PaginationParams pParams);
        Task<PagedList<TicketDto>> GetTickets(PaginationParams pParams, string userId);
        Task<TicketDto> GetTicket(int id, string userId);
        Task<TicketDto?> CreateTicket(TicketPostDto ticketPostDto, string userId);
        Task<bool> DeleteTicket(int id, string userId);
    }
}