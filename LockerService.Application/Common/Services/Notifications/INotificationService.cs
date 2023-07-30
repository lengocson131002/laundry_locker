namespace LockerService.Application.Common.Services.Notifications;

public interface INotificationService
{
    public Task NotifyAsync(Notification notification);
}