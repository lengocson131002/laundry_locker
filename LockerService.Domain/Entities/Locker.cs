using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
    
    public Store Store { get; set; } = default!;
    
    public long StoreId { get; set; }

    public string? MacAddress { get; set; }
    
    public string? IpAddress { get; set; }
    
    [JsonIgnore]
    public IList<Hardware> Hardwares { get; private set; } = new List<Hardware>();
    
    [JsonIgnore]
    public IList<LockerTimeline> Timelines { get; private set; } = new List<LockerTimeline>();
    
    [JsonIgnore]
    public IList<StaffLocker> StaffLockers { get; private set; } = new List<StaffLocker>();


    [JsonIgnore]
    public IList<Account> Staffs { get; set; } = new List<Account>();

    [JsonIgnore]
    public IList<Box> Boxes { get; set; } = new List<Box>();

    [JsonIgnore]
    public IList<Order> Orders { get; set; } = new List<Order>();

    [JsonIgnore]
    public IList<LockerOrderType> OrderTypes { get; set; } = new List<LockerOrderType>();
    
    public bool CanUpdateStatus => !Equals(LockerStatus.Initialized, Status) 
                                   && !Equals(LockerStatus.Disconnected, Status);

    public bool CanSupportOrderType(OrderType orderType)
    {
        return OrderTypes.Any(lockerOrderType => Equals(lockerOrderType.OrderType, orderType));
    }
}