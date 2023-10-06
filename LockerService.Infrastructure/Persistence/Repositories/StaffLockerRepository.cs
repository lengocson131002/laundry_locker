using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

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

    public async Task<IList<Account>> GetStaffs(long lockerId)
    {
        return await _dbContext.StaffLockers
            .Where(item => Equals(item.LockerId, lockerId))
            .Select(item => item.Staff)
            .ToListAsync();
    }

    public async Task<IList<Locker>> GetLocker(long staffId)
    {
        return await _dbContext.StaffLockers
            .Where(item => Equals(item.LockerId, staffId))
            .Select(item => item.Locker)
            .ToListAsync();
    }
}