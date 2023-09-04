using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.Projectables;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Order")]
public class Order : BaseAuditableEntity
{
    [Key] public long Id { get; set; }

    public OrderType Type { get; set; }

    public string? PinCode { get; set; }

    public DateTimeOffset? PinCodeIssuedAt { get; set; }

    // Sender info
    public long SendBoxId { get; set; }

    public Box SendBox { get; set; } = default!;

    public long SenderId { get; set; }

    public Account Sender { get; set; } = default!;

    // Receiver info
    public long ReceiveBoxId { get; set; }

    public Box ReceiveBox { get; set; } = default!;

    public long? ReceiverId { get; set; }

    public Account? Receiver { get; set; }

    public DateTimeOffset? ReceiveAt { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Initialized;

    public OrderCancelReason? CancelReason { get; set; }

    public long LockerId { get; set; }

    public Locker Locker { get; set; } = default!;

    public long? StaffId { get; set; }

    public Account? Staff { get; set; }

    public decimal? Price { get; set; }

    public float? ExtraCount { get; set; }

    public decimal? ExtraFee { get; set; }

    public decimal? Discount { get; set; }

    public string? Description { get; set; }

    public long? BillId { get; set; }

    public long? DeliveryAddressId { get; set; }
    public Location? DeliveryAddress { get; set; }

    public Bill? Bill { get; set; }

    public IList<OrderTimeline> Timelines { get; set; } = new List<OrderTimeline>();

    public IList<OrderDetail> Details { get; set; } = new List<OrderDetail>();

    public IList<LaundryItem> Items { get; set; } = new List<LaundryItem>();

    [Projectable]
    public decimal? TotalPrice => Price != null
        ? Price.Value + (decimal)(ExtraCount ?? 0) * (ExtraFee ?? 0) - (Discount ?? 0)
        : null;

    [Projectable] 
    public bool IsFinished => OrderStatus.Canceled.Equals(Status) || OrderStatus.Completed.Equals(Status);

    [Projectable] 
    public bool IsCompleted => OrderStatus.Completed.Equals(Status);

    [Projectable] 
    public bool IsInitialized => OrderStatus.Initialized.Equals(Status);

    [Projectable] 
    public bool IsReserved => OrderStatus.Reserved.Equals(Status);
    
    [Projectable] 
    public bool IsWaiting => OrderStatus.Waiting.Equals(Status);

    [Projectable] 
    public bool IsProcessing => OrderStatus.Processing.Equals(Status);

    [Projectable] 
    public bool IsReturned => OrderStatus.Returned.Equals(Status);

    [Projectable] 
    public bool IsOvertime => OrderStatus.Overtime.Equals(Status);

    /**
     * Order status, at which staff can process laundry order 
     * WAITING || RETURNED
     */
    [Projectable] 
    public bool CanProcess => OrderType.Laundry.Equals(Type) && (IsWaiting || IsReturned);

    /**
     * Order status, at which customer can checkout 
     * OVERTIME
     * WAITING for Storage
     * RETURNED for Laundry
     */
    [Projectable]
    public bool CanCheckout => (OrderType.Storage.Equals(Type) && IsWaiting)
                               || (OrderType.Laundry.Equals(Type) && IsReturned)
                               || IsOvertime;

    public bool UpdatedInfo => OrderType.Storage.Equals(Type) || Details.Any() && Details.All(item => item.Quantity != null);

    public bool UpdatedItems => OrderType.Storage.Equals(Type) || Items.Any();

    /**
     * Order status, which is current active
     * INITIALIZED || WAITING || RESERVED || PROCESSING || RETURNED || OVERTIME
     */
    [Projectable]
    public bool IsActive => !IsFinished;

    /**
     * Order status, which takes box in the locker
     * INITIALIZED || WAITING || RESERVED || RETURNED || OVERTIME
     */
    [Projectable]
    public bool IsBusyOrder => !IsFinished && !IsProcessing;

    [Projectable]
    public bool DeliverySupported => DeliveryAddressId != null;
}