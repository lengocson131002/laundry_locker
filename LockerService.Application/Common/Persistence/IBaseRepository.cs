using System.Linq.Expressions;

namespace LockerService.Application.Common.Persistence;

public interface IBaseRepository<T> where T : class
{
    Task<IQueryable<T>> GetAllAsync();

    Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> predicate);

    Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        List<Expression<Func<T, object>>> includes = null,
        bool disableTracking = true);

    Task<T?> GetByIdAsync(object id);

    Task<T> AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);

    Task AddRange(IEnumerable<T> entities);

    Task DeleteRange(IEnumerable<T> entities);

    Task DeleteAsync(object id);

    bool Any();
}