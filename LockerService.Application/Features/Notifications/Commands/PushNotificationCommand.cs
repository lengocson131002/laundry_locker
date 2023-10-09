using LockerService.Application.Features.Notifications.Models;

namespace LockerService.Application.Features.Notifications.Commands;

public class PushNotificationCommand : IRequest<NotificationModel>
{
    public NotificationType Type { get; set; }
    
    public long AccountId { get; set; }
    
}