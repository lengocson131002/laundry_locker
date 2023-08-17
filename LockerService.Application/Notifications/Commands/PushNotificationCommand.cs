namespace LockerService.Application.Notifications.Commands;

public class PushNotificationCommand : IRequest
{
    public NotificationType Type { get; set; }
    
    public long AccountId { get; set; }
}