using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Service")]
public class Service : BaseAuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string Image { get; set; } = default!;
    
    public decimal Price { get; set; }
    
    public string? Unit { get; set; }
    
    public string? Description { get; set; }

    public ServiceStatus Status { get; set; } = ServiceStatus.Active;
    
    public bool IsActive => ServiceStatus.Active.Equals(Status);
}