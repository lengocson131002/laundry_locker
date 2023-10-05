namespace LockerService.Application.Common.Services;

public interface ILockersService
{
    public Task CheckLockerBoxAvailability(Locker locker);
    
}