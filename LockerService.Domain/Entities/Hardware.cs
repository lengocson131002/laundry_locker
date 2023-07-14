using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerService.Domain.Entities;

[Table("Hardware")]
public class Hardware : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Code { get; set; }
    
    public string? Brand { get; set; }
    
    public decimal? Price { get; set; }
    
    public string? Description { get; set; }
    
    public long LockerId { get; set; }

    public Locker Locker { get; set; } = default!;

    public bool IsActive => DeletedAt == null;
}