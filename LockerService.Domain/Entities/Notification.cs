using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table(("Notification"))]
public class Notification
{
    [Key]
    public int Id { get; set; }
    
    public int AccountId { get; set; }

    public Account Account { get; set; } = default!;
    
    public NotificationType Type { get; set; }
    
    public EntityType EntityType { get; set; }

    public string? ReferenceId { get; set; }

    public string Content { get; set; } = default!;
    
    public string? Data { get; set; }

    public DateTimeOffset? ReadAt { get; set; }

    public bool IsRead => ReadAt != null;
}