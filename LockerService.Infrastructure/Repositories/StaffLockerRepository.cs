using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LockerService.Infrastructure.Repositories;

public class StaffLockerRepository : BaseRepository<StaffLocker>, IStaffLockerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StaffLockerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsManaging(long staffId, long lockerId)
    {
        var staffLocker = await _dbContext.StaffLockers
            .Where(item => Equals(item.StaffId, staffId) && Equals(item.LockerId, lockerId))
            .FirstOrDefaultAsync();
        return staffLocker != null;
    }

}