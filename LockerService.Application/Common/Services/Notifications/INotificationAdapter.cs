using LockerService.Application.Common.Services.Notifications.Models;

namespace LockerService.Application.Common.Services.Notifications;

public interface INotificationAdapter
{
    public Task<WebNotification> ToWebNotification(Notification notification, string connectionId);

    public Task<ZaloZnsNotification> ToZaloZnsNotification(Notification notification);

    public Task<FirebaseNotification> ToFirebaseNotification(Notification notification, string deviceToken, DeviceType? deviceType = null);
}