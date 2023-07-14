using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("OrderTimeline")]
public class OrderTimeline : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public long OrderId { get; set; }

    public Order Order { get; set; } = default!;
    
    public OrderStatus Status { get; set; }
    
    public OrderStatus? PreviousStatus { get; set; }
    
    [Column(TypeName = "text")]
    public string? Description { get; set; }
}