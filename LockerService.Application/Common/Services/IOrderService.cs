namespace LockerService.Application.Common.Services;

public interface IOrderService
{
    Task CancelExpiredOrder(long orderId, DateTimeOffset time);
    
}