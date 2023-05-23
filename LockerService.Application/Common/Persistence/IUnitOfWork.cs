using LockerService.Domain.Entities;

namespace LockerService.Application.Common.Persistence;

public interface IUnitOfWork : IBaseUnitOfWork
{
    IBaseRepository<Account> AccountRepository { get; }
}