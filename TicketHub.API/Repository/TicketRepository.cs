using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TicketHub.API.Models;
using TicketHub.API.Pagination;

namespace TicketHub.API.Repository
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationContext context) : base(context) { }

        public async Task<PagedList<Ticket>> GetTicketsWithEvent(PaginationParams pParams)
        {
            var query = _dbSet.AsNoTracking()
                .Include(t => t.Event)
                    .ThenInclude(e => e!.Category)
                .OrderBy(t => t.PucharseDate)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pParams.PageNumber - 1) * pParams.PageSize)
                .Take(pParams.PageSize)
                .ToListAsync();

            return new PagedList<Ticket>(items, totalCount, pParams.PageNumber, pParams.PageSize);
        }

        public async Task<PagedList<Ticket>> GetTicketsWithEvent(PaginationParams pParams, string userId)
        {
            var query = _dbSet.AsNoTracking()
                .Where(t => t.UserId == userId)
                .Include(t => t.Event)
                    .ThenInclude(e => e!.Category)
                .OrderBy(t => t.PucharseDate)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pParams.PageNumber - 1) * pParams.PageSize)
                .Take(pParams.PageSize)
                .ToListAsync();

            return new PagedList<Ticket>(items, totalCount, pParams.PageNumber, pParams.PageSize);
        }

        public async Task<Ticket?> GetTicketWithEvent(int id, string userId) =>
            await _dbSet.AsNoTracking()
                .Include(t => t.Event)
                    .ThenInclude(e => e!.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }
}