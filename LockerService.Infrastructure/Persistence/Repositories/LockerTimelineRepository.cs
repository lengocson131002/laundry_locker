using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class LockerTimelineRepository : BaseRepository<LockerTimeline>, ILockerTimelineRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public LockerTimelineRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}