namespace LockerService.Application.Common.Services.Notifications;

public interface INotificationProvider
{
    void Attach(NotificationType type, INotificationService notificationService);

    void Detach(NotificationType type, INotificationService notificationService);

    Task NotifyAsync(Notification notification);
}