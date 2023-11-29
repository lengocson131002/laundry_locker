using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence.Contexts;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.Persistence.Repositories;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateOrderPinCode(int length = 6)
    {
        while (true)
        {
            var pinCode = TokenUtils.GeneratePinCode(length);
            var order = await GetOrderByPinCode(pinCode).FirstOrDefaultAsync();
            if (order == null)
            {
                return pinCode;
            }
        }
    }

    public IQueryable<Order> GetOrderByPinCode(string pinCode)
    {
        return _dbContext.Orders
            .Where(order => pinCode.Equals(order.PinCode))
            .Where(order => order.IsActive);
    }

    public Task<int> CountActiveOrders(long customerId)
    {
        return _dbContext.Orders
            .Where(order => order.SenderId == customerId || order.ReceiverId == customerId)
            .Where(order => order.IsActive)
            .CountAsync();
    }

    public IQueryable<Order> GetOrders(DateTimeOffset? from, DateTimeOffset? to)
    {
        return _dbContext.Orders.Where(order =>
            (from == null || order.CreatedAt >= from) && (to == null || order.CreatedAt <= to));
    }

    public IQueryable<Order> GetOrders(long? storeId = null, long? lockerId = null, DateTimeOffset? from = null,
        DateTimeOffset? to = null)
    {
        var query = GetOrders(from, to);
        if (storeId != null) query = query.Where(order => order.Locker.StoreId == storeId);

        if (lockerId != null) query = query.Where(order => order.LockerId == lockerId);

        return query;
    }

    public IQueryable<Order> GetOrderInformation(long id)
    {
        return Get(order => order.Id == id)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Locker.Location)
            .Include(order => order.Locker.Location.Ward)
            .Include(order => order.Locker.Location.District)
            .Include(order => order.Locker.Location.Province)
            .Include(order => order.Details)
            .ThenInclude(detail => detail.Service);
    }

    public IQueryable<Order> GetOrderByPinCode(string pinCode, long lockerId)
    {
        return _dbContext.Orders
            .Where(order => pinCode.Equals(order.PinCode) && Equals(order.LockerId, lockerId));
    }

    public IQueryable<Order> GetOvertimeOrders()
    {
        var now = DateTimeOffset.UtcNow;
        return _dbContext.Orders
            .Where(order => (order.IsWaiting || order.IsReturned) && now >= order.IntendedOvertime);
    }
    
}