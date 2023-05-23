using LockerService.Application.Common.Persistence;
using LockerService.Domain.Entities;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class UnitOfWork :  BaseUnitOfWork, IUnitOfWork
{
    private IBaseRepository<Account>? _accountRepository;
    
    private readonly ApplicationDbContext _dbContext;
    
    public UnitOfWork(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public IBaseRepository<Account> AccountRepository =>
        _accountRepository ??= new BaseRepository<Account>(_dbContext);
    
}