using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("LandryItem")]
public class LaundryItem : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public ClothType Type { get; set; }

    public string Image { get; set; } = default!;
    
    public string? Description { get; set; }
    
    public long OrderId { get; set; }

    public Order Order { get; set; } = default!;

}