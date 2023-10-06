namespace LockerService.Application.Features.Orders.Models;

public class LaundryItemResponse : BaseAuditableEntityResponse
{
    public long Id { get; set; }
    
    public ClothType Type { get; set; }

    public string Image { get; set; } = default!;
    
    public string? Description { get; set; }
    
}