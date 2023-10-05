using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EntityFrameworkCore.Projectables;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Box")]
public class Box : BaseAuditableEntity
{
    [Key] 
    public long Id { get; set; }
    
    public int Number { get; set; }
    
    public bool IsActive { get; set; }
    
    public long LockerId { get; set; }

    public Locker Locker { get; set; } = default!;

    [JsonIgnore]
    [InverseProperty(nameof(Order.SendBox))]
    public IList<Order> SendOrders { get; set; } = new List<Order>();

    [JsonIgnore]
    [InverseProperty(nameof(Order.ReceiveBox))]
    public IList<Order> ReceiveOrders { get; set; } = new List<Order>();
    
    [NotMapped]
    [JsonIgnore]
    public Order? LastOrder { get; set; }

    [Projectable]
    public bool IsAvailable => IsActive && (LastOrder == null || !LastOrder.IsBusyOrder);

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
