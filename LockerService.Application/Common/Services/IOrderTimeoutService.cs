namespace LockerService.Application.Common.Services;

public interface IOrderTimeoutService
{
    Task CancelExpiredOrder(long orderId, DateTimeOffset time);
}