namespace LockerService.Application.Common.Services.Notifications;

public interface INotifier
{
    Task NotifyAsync(Notification notification);
}