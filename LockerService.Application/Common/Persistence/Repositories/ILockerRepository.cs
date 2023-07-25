namespace LockerService.Application.Common.Persistence.Repositories;

public interface ILockerRepository : IBaseRepository<Locker>
{
    Task<Box?> FindAvailableBox(long lockerId);
    
    Task<IList<Box>> FindAvailableBoxes(long lockerId);

    Task<IList<Box>> GetAllBoxes(long lockerId);

    Task<Locker?> FindByName(string name);

    Task<Locker?> FindByCode(string code);
}