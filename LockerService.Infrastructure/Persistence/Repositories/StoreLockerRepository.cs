using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class StoreLockerRepository : BaseRepository<StoreService>, IStoreServiceRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public StoreLockerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoreService?> GetStoreService(long storeId, long serviceId)
    {
        return await _dbContext.StoreServices
            .Where(item => item.StoreId == storeId && item.ServiceId == serviceId)
            .Include(item => item.Service)
            .Include(item => item.Store)
            .FirstOrDefaultAsync();
    }
}