namespace LockerService.Application.Common.Persistence.Repositories;

public interface ILockerRepository : IBaseRepository<Locker>
{
    Task<int?> FindAvailableBox(int lockerId);
    
    Task<IList<int>> FindAvailableBoxes(int lockerId);

    Task<IList<BoxStatus>> GetAllBoxes(int lockerId);

    Task<Locker?> FindByMac(string mac);

    Task<Locker?> FindByName(string name);
}