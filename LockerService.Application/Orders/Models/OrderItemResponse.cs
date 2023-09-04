namespace LockerService.Application.Orders.Models;

public class OrderItemResponse : BaseAuditableEntityResponse
{
    public long Id { get; set; }
    
    public ServiceResponse Service { get; set; } = default!;
    
    public float? Quantity { get; set; }
    
    public decimal? Price { get; set; }
    
}