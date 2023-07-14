using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class StaffLockerRepository : BaseRepository<StaffLocker>, IStaffLockerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StaffLockerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}