using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;

namespace LockerService.Infrastructure.Repositories;

public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}