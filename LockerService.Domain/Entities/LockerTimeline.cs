using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;
using LockerService.Domain.Events;

namespace LockerService.Domain.Entities;

[Table("LockerTimeline")]
public class LockerTimeline
{
    public int Id { get; set; }
    
    public int LockerId { get; set; }

    public Locker Locker { get; set; } = default!;
    
    public LockerEvent? Event { get; set; }

    public LockerStatus? Status { get; set; }
    
    public LockerStatus? PreviousStatus { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string? Data { get; set; }
    
    public string? Description { get; set; }
    
    public int? ErrorCode { get; set; }
    
    public string? Error { get; set; }
    
    public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;

}