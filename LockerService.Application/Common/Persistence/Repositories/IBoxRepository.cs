namespace LockerService.Application.Common.Persistence.Repositories;

public interface IBoxRepository : IBaseRepository<Box>
{
    Task<Box?> FindBox(long lockerId, int number);
    
    Task<Box?> FindAvailableBox(long lockerId);
    
    Task<IList<Box>> FindAvailableBoxes(long lockerId);

    Task<IList<Box>> GetAllBoxes(long lockerId);
}