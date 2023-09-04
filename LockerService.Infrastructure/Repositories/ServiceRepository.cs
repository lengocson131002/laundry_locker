using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class ServiceRepository : BaseRepository<Service>, IServiceRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public ServiceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Service?> GetStoreService(long storeId, long serviceId)
    {
        return _dbContext.Services
            .Where((service => Equals(service.Id, serviceId) && Equals(service.StoreId, storeId)))
            .FirstOrDefaultAsync();
    }
}