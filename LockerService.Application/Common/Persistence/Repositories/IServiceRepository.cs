namespace LockerService.Application.Common.Persistence.Repositories;

public interface IServiceRepository : IBaseRepository<Service>
{
    public Task<Service?> GetStoreService(long storeId, long serviceId);
    
}