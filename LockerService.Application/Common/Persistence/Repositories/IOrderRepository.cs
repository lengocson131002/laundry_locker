namespace LockerService.Application.Common.Persistence.Repositories;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<string> GenerateOrderPinCode(int length = 6);

    IQueryable<Order> GetOrderByPinCode(string pinCode);

    Task<int> CountActiveOrders(long customerId);

    IQueryable<Order> GetOrders(DateTimeOffset? from = null, DateTimeOffset? to = null);
    
    IQueryable<Order> GetOrders(long? storeId = null, long? lockerId = null, DateTimeOffset? from = null, DateTimeOffset? to = null);

    IQueryable<Order> GetOrder(long id);
    
    IQueryable<Order> GetOrderByPinCode(string pinCode, long lockerId);

    IQueryable<Order> GetOvertimeOrders(int maxTimeInHours);
}