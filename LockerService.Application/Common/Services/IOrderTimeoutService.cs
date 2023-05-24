namespace LockerService.Application.Common.Services;

public interface IOrderTimeoutService
{
    Task CancelExpiredOrder(int orderId, DateTimeOffset time);
}