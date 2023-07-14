namespace LockerService.Application.Orders.Models;

public class OrderResponse
{
    public long Id { get; set; }
    
    public string? PinCode { get; set; }
    
    public DateTimeOffset? PinCodeIssuedAt { get; set; } 
    
    public int SendBox { get; set; }
    
    public string SendPhone { get; set; } = default!;

    public int SenderId { get; set; }

    public int ReceiveBox { get; set; }
    
    public string? ReceivePhone { get; set; }

    public int? ReceiverId { get; set; }

    public DateTimeOffset? ReceiveAt { get; set; }

    public OrderStatus Status { get; set; }
    
    public int LockerId { get; set; }
    
    public string LockerName { get; set; } = default!;

    public decimal Price { get; set; }

    public float? ExtraCount { get; set; }

    public decimal? ExtraFee { get; set; }

    public decimal? Discount { get; set; }
    
    public string? Description { get; set; }
    
    public DateTimeOffset? CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
}