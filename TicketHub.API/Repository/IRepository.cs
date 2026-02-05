using System.Linq.Expressions;
using TicketHub.API.Pagination;

namespace TicketHub.API.Repository
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> Get();
        Task<T?> GetById(int id);
        Task Create(T create);
        void Update(T update);
        void Delete(T delete);
        Task Save();
        Task Any(Expression<Func<T, bool>> expression);
        Task<T?> Find(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>?> Get(Expression<Func<T, bool>> predicate);
        Task<int> Count(Expression<Func<T, bool>> predicate);
        Task<PagedList<T>> GetPaged(PaginationParams pParams, Expression<Func<T, bool>>? filter = null);
    }
}