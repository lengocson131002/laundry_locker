using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Order")]
public class Order : BaseAuditableEntity
{
    [Key]
    public int Id { get; set; }

    public OrderType Type { get; set; } 

    public string? PinCode { get; set; }

    public DateTimeOffset? PinCodeIssuedAt { get; set; } 
    
    // Sender info
    public int SendBox { get; set; }
    
    public string SendPhone { get; set; } = default!;

    public int SenderId { get; set; }

    public Account Sender { get; set; } = default!;
    
    // Receiver info
    public int ReceiveBox { get; set; }
    
    public string? ReceivePhone { get; set; }

    public int? ReceiverId { get; set; }

    public Account? Receiver { get; set; }
    
    public DateTimeOffset? ReceiveAt { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Initialized;
    
    public int LockerId { get; set; }
    
    public Locker Locker { get; set; } = default!;
    
    public int? StaffId { get; set; }

    public Account? Staff { get; set; }
    
    public decimal Price { get; set; }

    public float? ExtraCount { get; set; }

    public decimal? ExtraFee { get; set; }

    public decimal? Discount { get; set; }
    
    public string? Description { get; set; }
    
    public int? BillId { get; set; }
    
    public Bill? Bill { get; set; }
    
    public IList<OrderTimeline> Timelines { get; set; } = new List<OrderTimeline>();

    public IList<OrderDetail> Details { get; set; } = new List<OrderDetail>();

    public bool CanProcess => OrderStatus.Waiting.Equals(Status) || OrderStatus.Returned.Equals(Status);

    public bool IsFinished => OrderStatus.Canceled.Equals(Status) || OrderStatus.Completed.Equals(Status);

    public bool IsInitialized => OrderStatus.Initialized.Equals(Status);

    public bool IsWaiting => OrderStatus.Waiting.Equals(Status);

    public bool IsProcessing => OrderStatus.Processing.Equals(Status);

    public bool IsReturned => OrderStatus.Returned.Equals(Status);
}