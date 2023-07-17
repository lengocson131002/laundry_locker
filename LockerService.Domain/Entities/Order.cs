using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Order")]
public class Order : BaseAuditableEntity
{
    [Key] public int Id { get; set; }

    public string? PinCode { get; set; }

    public DateTimeOffset? PinCodeIssuedAt { get; set; }

    public int SendBoxOrder { get; set; }

    public string OrderPhone { get; set; } = default!;
    public int SenderId { get; set; }
    public Account Sender { get; set; }
    public string? ReceivePhone { get; set; }
    public int? ReceiverId { get; set; }
    public Account? Receiver { get; set; }

    public int ReceiveBoxOrder { get; set; }

    public DateTimeOffset? ReceiveTime { get; set; }

    public DateTimeOffset? ActualReceiveTime { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Initialized;

    public int ServiceId { get; set; }

    public Service Service { get; set; } = default!;

    public int LockerId { get; set; }

    public Locker Locker { get; set; } = default!;

    public int? StaffId { get; set; }

    public int? MemberId { get; set; }

    public double? Amount { get; set; }

    public double? Fee { get; set; }

    public string? Description { get; set; }

    public IList<OrderTimeline> Timelines { get; set; } = new List<OrderTimeline>();

    public bool CanProcess => OrderStatus.Waiting.Equals(Status) || OrderStatus.Returned.Equals(Status);

    public bool IsFinished => OrderStatus.Canceled.Equals(Status) || OrderStatus.Completed.Equals(Status);

    public bool IsInitialized => OrderStatus.Initialized.Equals(Status);

    public bool IsWaiting => OrderStatus.Waiting.Equals(Status);

    public bool IsProcessing => OrderStatus.Processing.Equals(Status);

    public bool IsReturned => OrderStatus.Returned.Equals(Status);
}