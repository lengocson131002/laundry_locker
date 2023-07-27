namespace LockerService.Application.Common.Services;

public interface IOrderService
{
    Task CalculateFree(Order order);
    
    Task CancelExpiredOrder(long orderId, DateTimeOffset time);
    
}