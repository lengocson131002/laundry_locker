using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class StoreRepository : BaseRepository<Store>, IStoreRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StoreRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}