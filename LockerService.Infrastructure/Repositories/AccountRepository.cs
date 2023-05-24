using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class AccountRepository : BaseRepository<Account>, IAccountRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public AccountRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}