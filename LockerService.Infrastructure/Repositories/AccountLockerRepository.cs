using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class AccountLockerRepository : BaseRepository<StaffLocker>, IAccountLockerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AccountLockerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}