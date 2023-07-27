namespace LockerService.Application.Common.Persistence.Repositories;

public interface IStaffLockerRepository : IBaseRepository<StaffLocker>
{
    public Task<bool> IsManaging(long staffId, long lockerId);
    
}