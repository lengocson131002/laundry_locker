namespace LockerService.Application.Common.Persistence.Repositories;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<string> GenerateOrderPinCode(int length = 6);

    IQueryable<Order> GetOrderByPinCode(string pinCode);

    Task<int> CountActiveOrders(long customerId);
}