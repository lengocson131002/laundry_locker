using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IList<OrderDetail>> GetOrderDetails(long orderId)
    {
        return await _dbContext.OrderDetails.Where(item => item.OrderId == orderId)
            .Include(item => item.Service)
            .ToListAsync();
    }
}