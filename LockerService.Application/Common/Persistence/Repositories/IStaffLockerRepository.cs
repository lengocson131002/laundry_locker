namespace LockerService.Application.Common.Persistence.Repositories;

public interface IStaffLockerRepository : IBaseRepository<StaffLocker>
{
    public Task<bool> IsManaging(long staffId, long lockerId);

    public Task<IList<Account>> GetStaffs(long lockerId);
    
    public Task<IList<Locker>> GetLocker(long staffId);
}