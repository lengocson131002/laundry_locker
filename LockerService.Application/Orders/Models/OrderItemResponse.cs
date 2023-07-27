namespace LockerService.Application.Orders.Models;

public class OrderItemResponse
{
    public long Id { get; set; }
    
    public ServiceResponse Service { get; set; } = default!;
    
    public float? Quantity { get; set; }
    
    public decimal? Price { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
}