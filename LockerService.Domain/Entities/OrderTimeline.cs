using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("OrderTimeline")]
public class OrderTimeline
{
    [Key]
    public int Id { get; set; }
    
    public int OrderId { get; set; }

    public Order Order { get; set; } = default!;
    
    public OrderStatus Status { get; set; }
    
    public OrderStatus? PreviousStatus { get; set; }
    
    public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;
    
    public string? Description { get; set; }
}