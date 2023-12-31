using LockerService.Application.Features.Customers.Models;

namespace LockerService.Application.Features.Lockers.Models;

public class BoxResponse
{
    public long Id { get; set; }
    
    public int Number { get; set; }
    
    public bool IsActive { get; set; }
    
    public long LockerId { get; set; }
    
    public BoxOrderResponse? LastOrder { get; set; }

    public bool IsAvailable { get; set; }
}

public class BoxOrderResponse : BaseAuditableEntityResponse
{
    public long Id { get; set; }
    
    public string? PinCode { get; set; }
    
    public OrderType Type { get; set; }
    
    public DateTimeOffset? PinCodeIssuedAt { get; set; }

    public CustomerResponse Sender { get; set; } = default!;

    public CustomerResponse Receiver { get; set; } = default!;

    public DateTimeOffset? ReceiveAt { get; set; }

    public OrderStatus Status { get; set; }

    public decimal Price { get; set; }

    public float? ExtraCount { get; set; }

    public decimal? ExtraFee { get; set; }

    public decimal? Discount { get; set; }
    
    public string? Description { get; set; }
}