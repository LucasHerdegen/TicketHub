using Microsoft.EntityFrameworkCore;
using TicketHub.API.Models;
using TicketHub.API.Pagination;

namespace TicketHub.API.Repository
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(ApplicationContext context) : base(context) { }

        public async Task<PagedList<Event>> GetEventsWithCategory(PaginationParams pParams)
        {
            var query = _dbSet.AsNoTracking()
                .Include(e => e.Category)
                .OrderBy(e => e.Date)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pParams.PageNumber - 1) * pParams.PageSize)
                .Take(pParams.PageSize)
                .ToListAsync();

            return new PagedList<Event>(items, totalCount, pParams.PageNumber, pParams.PageSize);
        }

        public async Task<Event?> GetEventByIdWithCategory(int id) =>
            await _dbSet.AsNoTracking()
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

    }
}