using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Common.Persistence.Repositories;

public interface ISettingRepository : IBaseRepository<Setting>
{
    /*
     * Create if not found
     */
    public Task<T> GetSettings<T>() where T : ISetting, new();
}