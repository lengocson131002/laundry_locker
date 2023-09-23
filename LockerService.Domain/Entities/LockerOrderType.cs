using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("LockerOrderType")]
public class LockerOrderType
{
    [Key]
    [DatabaseGenerated((DatabaseGeneratedOption.Identity))]
    public long Id { get; set; }
    
    public long LockerId { get; set; }

    public Locker Locker { get; set; } = default!;
    
    public OrderType OrderType { get; set; }

    public LockerOrderType()
    {
        
    }
    
    public LockerOrderType(long lockerId, OrderType type)
    {
        LockerId = lockerId;
        OrderType = type;
    }
    
    public LockerOrderType(Locker locker, OrderType type)
    {
        Locker = locker;
        OrderType = type;
    }
    
    public LockerOrderType(OrderType type)
    {
        OrderType = type;
    }
}