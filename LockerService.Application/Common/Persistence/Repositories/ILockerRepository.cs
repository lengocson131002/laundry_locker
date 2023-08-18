namespace LockerService.Application.Common.Persistence.Repositories;

public interface ILockerRepository : IBaseRepository<Locker>
{
    Task<Locker?> FindByName(string name);

    Task<Locker?> FindByCode(string code);
}