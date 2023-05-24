using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class LockerTimelineRepository : BaseRepository<LockerTimeline>, ILockerTimelineRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public LockerTimelineRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}