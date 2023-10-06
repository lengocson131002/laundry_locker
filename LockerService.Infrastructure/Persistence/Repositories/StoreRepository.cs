using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class StoreRepository : BaseRepository<Store>, IStoreRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StoreRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

}