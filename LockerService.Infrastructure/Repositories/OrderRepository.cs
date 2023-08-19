using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Internal;

namespace LockerService.Infrastructure.Repositories;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    private const string AllowedCharacters = "0123456789";
    
    public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GenerateOrderPinCode(int length = 6)
    {
        while (true)
        {
            var pinCode = GeneratePinCode(length);
            var order =  await GetOrderByPinCode(pinCode).FirstOrDefaultAsync();
            if (order  == null) return pinCode;
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
            .Where(order => order.SenderId == customerId)
            .Where(order => order.IsActive)
            .CountAsync();
    }

    public IQueryable<Order> GetOrders(DateTimeOffset? from, DateTimeOffset? to)
    {
        return _dbContext.Orders.Where(order => (from == null || order.CreatedAt >= from) && (to == null || order.CreatedAt <= to));
    }

    public IQueryable<Order> GetOrders(long? storeId = null, long? lockerId = null, DateTimeOffset? from = null,
        DateTimeOffset? to = null)
    {
        var query = GetOrders(from, to);
        if (storeId != null)
        {
            query = query.Where(order => order.Locker.StoreId == storeId);
        }

        if (lockerId != null)
        {
            query = query.Where(order => order.LockerId == lockerId);
        }

        return query;
    }

    private string GeneratePinCode(int length)
    {
        var rand = new Random();
        
        var otp = string.Empty;

        for (var i = 0; i < length; i++)
        {
            otp += AllowedCharacters[rand.Next(0, AllowedCharacters.Length)];
        }

        return otp;
    }
}