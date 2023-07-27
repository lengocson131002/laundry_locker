using System.Data;
using LockerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace LockerService.Infrastructure.Repositories;

public class BaseUnitOfWork : IBaseUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private bool _disposed;

    public BaseUnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public int SaveChanges()
    {
        return _dbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public void Rollback()
    {
        foreach (var entry in _dbContext.ChangeTracker.Entries())
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
            }
    }

    public IDbTransaction BeginTransaction()
    {
        var transaction = _dbContext.Database.BeginTransaction();
        return transaction.GetDbTransaction();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
                _dbContext.Dispose();

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}