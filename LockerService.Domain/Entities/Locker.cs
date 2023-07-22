using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Locker")]
public class Locker : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;
    
    public string? Image { get; set; }
    
    public LockerStatus Status { get; set; } = LockerStatus.Initialized;
    
    public long LocationId { get; set; }

    public Location Location { get; set; } = default!;
    
    [Column(TypeName = "text")]
    public string? Description { get; set; }
    
    public IList<Hardware> Hardwares { get; private set; } = new List<Hardware>();

    public IList<LockerTimeline> Timelines { get; private set; } = new List<LockerTimeline>();

    public Store Store { get; set; } = default!;
    
    public long? StoreId { get; set; }

    public string? MacAddress { get; set; }
    
    public string? IpAddress { get; set; }

    public IList<Account> Staffs { get; set; } = new List<Account>();

    public IList<Box> Boxes { get; set; } = new List<Box>();

    public bool CanUpdateStatus => !Equals(LockerStatus.Initialized, Status) && !Equals(LockerStatus.Disconnected, Status);
}