namespace LockerService.Application.Common.Persistence.Repositories;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<string> GenerateOrderPinCode(int length = 6);

    Task<Order?> GetOrderByPinCode(string pinCode);
}