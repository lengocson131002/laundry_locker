namespace LockerService.Application.Notifications.Commands;

public class PushNotificationCommand : IRequest
{
    public NotificationType Type { get; set; }
    
    public long AccountId { get; set; }

    public string Data { get; set; } = default!;
}