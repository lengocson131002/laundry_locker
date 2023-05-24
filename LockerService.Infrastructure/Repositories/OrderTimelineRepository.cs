using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class OrderTimelineRepository : BaseRepository<OrderTimeline>, IOrderTimelineRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public OrderTimelineRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}