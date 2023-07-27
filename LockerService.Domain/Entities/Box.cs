using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Box")]
public class Box : BaseAuditableEntity
{
    [Key] 
    public long Id { get; set; }
    
    public int Number { get; set; }
    
    public int? PinNo { get; set; }

    public bool IsActive { get; set; }
    
    public long LockerId { get; set; }

    public Locker Locker { get; set; } = default!;

    [InverseProperty(nameof(Order.SendBox))]
    public IList<Order> SendOrders { get; set; } = new List<Order>();
    
    [InverseProperty(nameof(Order.ReceiveBox))]
    public IList<Order> ReceiveOrders { get; set; } = new List<Order>();
    
    [NotMapped]
    public Order? LastOrder { get; set; }
    
    [NotMapped]
    public bool IsAvailable => LastOrder == null || (!OrderStatus.Initialized.Equals(LastOrder.Status)
                                                     && !OrderStatus.Waiting.Equals(LastOrder.Status)
                                                     && !OrderStatus.Returned.Equals(LastOrder.Status));
    public Box()
    {
        IsActive = true;
    }

    public Box(long lockerId, int number)
    {
        IsActive = true;
        LockerId = lockerId;
        Number = number;
    }
}