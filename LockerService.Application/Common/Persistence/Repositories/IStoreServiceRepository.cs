namespace LockerService.Application.Common.Persistence.Repositories;

public interface IStoreServiceRepository : IBaseRepository<StoreService>
{
    public Task<StoreService?> GetStoreService(long storeId, long serviceId);
}