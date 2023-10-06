namespace LockerService.Application.Features.Notifications.Models;

public class NotificationModel : BaseAuditableEntityResponse
{
    public long Id { get; set; }
    
    public long AccountId { get; set; }

    public NotificationType Type { get; set; }
    
    public EntityType EntityType { get; set; }

    public string? ReferenceId { get; set; }

    public string Content { get; set; } = default!;
    
    public string? Data { get; set; }

    public DateTimeOffset? ReadAt { get; set; }
    
    public bool IsRead => ReadAt != null;
    
    public NotificationLevel Level { get; set; }
}