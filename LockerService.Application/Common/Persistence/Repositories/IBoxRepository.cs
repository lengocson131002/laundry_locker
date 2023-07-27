namespace LockerService.Application.Common.Persistence.Repositories;

public interface IBoxRepository : IBaseRepository<Box>
{
    Task<Box?> FindBox(long lockerId, int number);
}