namespace LockerService.Application.Common.Persistence.Repositories;

public interface ILockerRepository : IBaseRepository<Locker>
{
    Task<int?> FindAvailableBox(long lockerId);
    
    Task<IList<int>> FindAvailableBoxes(long lockerId);

    Task<IList<BoxResponse>> GetAllBoxes(long lockerId);

    Task<Locker?> FindByName(string name);

    Task<Locker?> FindByCode(string code);
}