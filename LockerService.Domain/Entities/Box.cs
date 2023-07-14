using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerService.Domain.Entities;

[Table("Box")]
public class Box : BaseAuditableEntity
{
    [Key] 
    public long Id { get; set; }
    
    public int Number { get; set; }
    
    public int? PinNo { get; set; }

    public bool IsActive { get; set; } = true;
    
    public long LockerId { get; set; }

    public Locker Locker { get; set; } = default!;
}