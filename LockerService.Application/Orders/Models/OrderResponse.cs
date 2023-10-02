namespace LockerService.Application.Orders.Models;

public class OrderResponse : BaseAuditableEntityResponse
{
    public long Id { get; set; }

    public OrderType Type { get; set; }

    public string? PinCode { get; set; }

    public DateTimeOffset? PinCodeIssuedAt { get; set; }

    public BoxResponse? SendBox { get; set; }

    public BoxResponse? ReceiveBox { get; set; }

    public CustomerResponse? Sender { get; set; }

    public CustomerResponse? Receiver { get; set; }

    public DateTimeOffset? ReceiveAt { get; set; }

    public OrderStatus Status { get; set; }

    public LockerResponse? Locker { get; set; }

    public decimal? Price { get; set; }

    public float? ExtraCount { get; set; }

    public decimal? ExtraFee { get; set; }

    public decimal? Discount { get; set; }

    public decimal? StoragePrice { get; set; }

    public decimal? ReservationFee { get; set; }

    public decimal? ShippingFee { get; set; }

    public decimal? TotalExtraFee { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Description { get; set; }

    public bool DeliverySupported { get; set; }
    
    public bool UpdatedInfo { get; set; }

    public DateTimeOffset? IntendedReceiveAt { get; set; }

    public DateTimeOffset? IntendedOvertime { get; set; }

    public OrderCancelReason? CancelReason { get; set; }
}