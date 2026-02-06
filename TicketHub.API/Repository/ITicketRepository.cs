using TicketHub.API.Models;
using TicketHub.API.Pagination;

namespace TicketHub.API.Repository
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<PagedList<Ticket>> GetTicketsWithEvent(PaginationParams pParams);
        Task<PagedList<Ticket>> GetTicketsWithEvent(PaginationParams pParams, string userId);
        Task<Ticket?> GetTicketWithEvent(int id, string userId);
    }
}