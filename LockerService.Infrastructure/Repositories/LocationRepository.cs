using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class LocationRepository : BaseRepository<Location>, ILocationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}