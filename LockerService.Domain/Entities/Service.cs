using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.Projectables;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Service")]
public class Service : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public string? Image { get; set; }
    
    public decimal Price { get; set; }
    
    public string? Unit { get; set; }
    
    public string? Description { get; set; }

    public ServiceStatus Status { get; set; } = ServiceStatus.Active;
    
    public bool IsActive => ServiceStatus.Active.Equals(Status);
    
    // Service without StoreId is standard service
    // which created by System Administrator
    // used for other stores' usage and config price (if needed)
    public long? StoreId { get; set; }

    public Store? Store { get; set; } = default!;       

    [Projectable] 
    public bool IsStandard => StoreId == null;
    
    public IList<StoreService> StoreServices { get; set; } = new List<StoreService>();
    
}