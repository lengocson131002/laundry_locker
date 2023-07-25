using System.Linq.Expressions;
using LockerService.Application.Common.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LockerService.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly DbContext _dbContext;

    public BaseRepository(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    // public Task<IQueryable<T>> GetAllAsync()
    // {
    //     return Task.FromResult(_dbContext.Set<T>().AsQueryable());
    // }
    //
    // public Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> predicate)
    // {
    //     return Task.FromResult(_dbContext.Set<T>().Where(predicate).AsQueryable());
    // }

    public Task<IQueryable<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<Expression<Func<T, object>>>? includes = null,
        bool disableTracking = false)
    {
        IQueryable<T> query = _dbContext.Set<T>();

        if (disableTracking) query = query.AsNoTracking();

        if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (predicate != null) query = query.Where(predicate);

        return Task.FromResult(orderBy != null ? orderBy(query).AsQueryable() : query.AsQueryable());
    }

    public virtual async Task<T?> GetByIdAsync(object id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbContext.Set<T>().Attach(entity);
        }
        await _dbContext.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbContext.Set<T>().Attach(entity);
        }

        _dbContext.Entry(entity).State = EntityState.Modified;

        _dbContext.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbContext.Set<T>().Attach(entity);
        }

        _dbContext.Set<T>().Remove(entity);

        return Task.CompletedTask;
    }

    public async Task AddRange(IEnumerable<T> entities)
    {
        var listEntities = entities.ToList();
        listEntities.ForEach(entity =>
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                _dbContext.Set<T>().Attach(entity);
            }
        });
        await _dbContext.Set<T>().AddRangeAsync(listEntities);
    }

    public Task DeleteRange(IEnumerable<T> entities)
    {
        var listEntities = entities.ToList();
        listEntities.ForEach(entity =>
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                _dbContext.Set<T>().Attach(entity);
            }
        });

        _dbContext.Set<T>().RemoveRange(listEntities);

        return Task.CompletedTask;
    }

    public async Task DeleteAsync(object id)
    {
        var entityToDelete = await _dbContext.Set<T>().FindAsync(id);

        if (entityToDelete != null)
        {
            await DeleteAsync(entityToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }

    public bool Any()
    {
        return _dbContext.Set<T>().Any();
    }
}