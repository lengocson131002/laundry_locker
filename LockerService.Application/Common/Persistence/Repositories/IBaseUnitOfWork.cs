using System.Data;

namespace LockerService.Application.Common.Persistence.Repositories;

public interface IBaseUnitOfWork
{
    Task<int> SaveChangesAsync();

    void Rollback();

    IDbTransaction BeginTransaction();
}