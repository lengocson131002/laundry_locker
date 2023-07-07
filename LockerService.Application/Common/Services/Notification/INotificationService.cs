using LockerService.Application.Common.Services.Notification.Data;

namespace LockerService.Application.Common.Services.Notification;

public interface INotificationService<T> where T : NotificationData
{
    public Task SendAsync(T data);
}