using System.Data;

namespace LockerService.Application.Common.Persistence;

public interface IBaseUnitOfWork
{
    Task<int> SaveChangesAsync();

    void Rollback();

    IDbTransaction BeginTransaction();
}