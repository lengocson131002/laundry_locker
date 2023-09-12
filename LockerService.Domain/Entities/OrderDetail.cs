using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerService.Domain.Entities;

[Table("OrderDetail")]
public class OrderDetail : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public long ServiceId { get; set; }

    public Service Service { get; set; } = default!;
    
    public float? Quantity { get; set; }
    
    public decimal Price { get; set; }
    
    public long OrderId { get; set; }

    public Order Order { get; set; } = default!;
    
    public string? Description { get; set; }
    
    public IList<LaundryItem> Items { get; set; } = new List<LaundryItem>();
}