using TicketHub.API.Models;
using TicketHub.API.Pagination;

namespace TicketHub.API.Repository
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<PagedList<Event>> GetEventsWithCategory(PaginationParams pParams);
        Task<Event?> GetEventByIdWithCategory(int id);
    }
}