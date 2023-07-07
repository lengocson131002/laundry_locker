namespace LockerService.Application.Orders.Models;

public class OrderItemResponse
{
    public int Id { get; set; }
    
    public ServiceResponse Service { get; set; } = default!;
    
    public float? Quantity { get; set; }
    
    public decimal? Price { get; set; }
    
    public int OrderId { get; set; }
}