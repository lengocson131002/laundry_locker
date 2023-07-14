using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;
using LockerService.Domain.Events;

namespace LockerService.Domain.Entities;

[Table("LockerTimeline")]
public class LockerTimeline : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public long LockerId { get; set; }

    public Locker Locker { get; set; } = default!;
    
    public LockerEvent? Event { get; set; }

    public LockerStatus? Status { get; set; }
    
    public LockerStatus? PreviousStatus { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string? Data { get; set; }
    
    [Column(TypeName = "text")]
    public string? Description { get; set; }
    
    public int? ErrorCode { get; set; }
    
    public string? Error { get; set; }
    
}