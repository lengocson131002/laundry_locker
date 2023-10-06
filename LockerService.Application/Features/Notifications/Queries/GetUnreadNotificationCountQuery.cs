using LockerService.Application.Features.Notifications.Models;

namespace LockerService.Application.Features.Notifications.Queries;

public class GetUnreadNotificationCountQuery : IRequest<UnreadNotificationCountResponse>
{
    public NotificationType? Type { get; set; }
    
    public EntityType? EntityType { get; set; }
    
    public DateTimeOffset? From { get; set; }
    
    public DateTimeOffset? To { get; set; }
}