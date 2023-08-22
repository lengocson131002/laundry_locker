namespace LockerService.Application.Notifications.Models;

public class NotificationModel
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

    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
    public bool Deleted { get; set; }
}