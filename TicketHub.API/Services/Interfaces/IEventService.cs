using TicketHub.API.DTOs.Event;
using TicketHub.API.Pagination;

namespace TicketHub.API.Services.Interfaces
{
    public interface IEventService
    {
        Task<PagedList<EventDto>> GetEvents(PaginationParams pParams);
        Task<EventDto?> GetEvent(int id);
        Task<EventDto?> CreateEvent(EventPostDto eventPostDto);
        Task<bool> UpdateEvent(EventPutDto eventPutDto);
        Task<bool> DeleteEvent(int id);
    }
}