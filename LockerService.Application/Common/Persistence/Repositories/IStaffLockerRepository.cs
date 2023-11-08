namespace LockerService.Application.Common.Persistence.Repositories;

public interface IStaffLockerRepository : IBaseRepository<StaffLocker>
{
    public Task<IList<Account>> GetStaffs(long lockerId);
}