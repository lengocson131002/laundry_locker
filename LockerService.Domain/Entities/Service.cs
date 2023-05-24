using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Service")]
public class Service : BaseAuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string Image { get; set; } = default!;
    
    public double? Fee { get; set; }
    
    public FeeType FeeType { get; set; }
    
    public int LockerId { get; set; }

    public Locker Locker { get; set; } = default!;
    
    public string? Unit { get; set; }
    
    public string? Description { get; set; }

    public bool IsActive => DeletedAt == null;
}