using LockerService.Application.Features.Services.Models;

namespace LockerService.Application.Features.Orders.Models;

public class OrderItemResponse : BaseAuditableEntityResponse
{
    public long Id { get; set; }
    
    public ServiceResponse Service { get; set; } = default!;
    
    public float? Quantity { get; set; }
    
    public decimal? Price { get; set; }

    public IList<LaundryItemResponse> Items { get; set; } = new List<LaundryItemResponse>();
}