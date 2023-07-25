using LockerService.Application.Customers.Models;

namespace LockerService.Application.Orders.Models;

public class OrderResponse
{
    public long Id { get; set; }
    
    public string? PinCode { get; set; }
    
    public DateTimeOffset? PinCodeIssuedAt { get; set; }

    public BoxResponse SendBox { get; set; } = default!;

    public BoxResponse ReceiveBox { get; set; } = default!;

    public CustomerResponse Sender { get; set; } = default!;

    public CustomerResponse Receiver { get; set; } = default!;

    public DateTimeOffset? ReceiveAt { get; set; }

    public OrderStatus Status { get; set; }

    public LockerResponse Locker { get; set; } = default!;

    public decimal Price { get; set; }

    public float? ExtraCount { get; set; }

    public decimal? ExtraFee { get; set; }

    public decimal? Discount { get; set; }
    
    public string? Description { get; set; }
    
    public DateTimeOffset? CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
}