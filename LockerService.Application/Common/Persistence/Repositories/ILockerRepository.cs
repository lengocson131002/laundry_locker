namespace LockerService.Application.Common.Persistence.Repositories;

public interface ILockerRepository : IBaseRepository<Locker>
{
    Task<int?> FindAvailableBox(long lockerId);
    
    Task<IList<int>> FindAvailableBoxes(long lockerId);

    Task<IList<BoxStatus>> GetAllBoxes(long lockerId);

    Task<Locker?> FindByMac(string mac);

    Task<Locker?> FindByName(string name);
}