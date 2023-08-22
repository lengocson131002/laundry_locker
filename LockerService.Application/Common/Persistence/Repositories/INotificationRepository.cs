namespace LockerService.Application.Common.Persistence.Repositories;

public interface INotificationRepository : IBaseRepository<Notification>
{
    public Task<Notification?> GetNotification(long accountId, long notificationId);
}