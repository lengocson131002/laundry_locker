using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.Projectables;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table(("Notification"))]
public class Notification : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public long AccountId { get; set; }

    public Account Account { get; set; } = default!;
    
    public NotificationType Type { get; set; }
    
    public EntityType EntityType { get; set; }

    public string? ReferenceId { get; set; }

    public string Content { get; set; } = default!;
    
    [Column(TypeName = "jsonb")]
    public string? Data { get; set; }

    public DateTimeOffset? ReadAt { get; set; }

    [Projectable]
    public bool IsRead => ReadAt != null;
}