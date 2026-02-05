using Microsoft.EntityFrameworkCore;
using TicketHub.API.Models;
using System.Linq.Expressions;
using TicketHub.API.Pagination;

namespace TicketHub.API.Repository
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> Get() =>
            await _dbSet.AsNoTracking().ToListAsync();

        public virtual async Task<T?> GetById(int id) =>
            await _dbSet.FindAsync(id);

        public virtual async Task Create(T entity) =>
            await _dbSet.AddAsync(entity);

        public virtual void Update(T entity) =>
            _dbSet.Update(entity);

        public virtual void Delete(T entity) =>
            _dbSet.Remove(entity);

        public async Task Save() =>
            await _context.SaveChangesAsync();

        public async Task Any(Expression<Func<T, bool>> expression) =>
            await _dbSet.AnyAsync(expression);

        public async Task<T?> Find(Expression<Func<T, bool>> expression) =>
            await _dbSet.FirstOrDefaultAsync(expression);

        public async Task<IEnumerable<T>?> Get(Expression<Func<T, bool>> expression) =>
            await _dbSet.AsNoTracking().Where(expression).ToListAsync();

        public async Task<int> Count(Expression<Func<T, bool>> expression) =>
            await _dbSet.CountAsync(expression);

        public async Task<PagedList<T>> GetPaged(PaginationParams pParams, Expression<Func<T, bool>>? filter = null)
        {
            var query = _dbSet.AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pParams.PageNumber - 1) * pParams.PageSize)
                .Take(pParams.PageSize)
                .ToListAsync();

            return new PagedList<T>(items, totalCount, pParams.PageNumber, pParams.PageSize);
        }

    }
}