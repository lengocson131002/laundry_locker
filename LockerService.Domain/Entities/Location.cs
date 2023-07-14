using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerService.Domain.Entities;

[Table("Location")]
public class Location : BaseAuditableEntity
{
    [Key]
    public int Id { get; set; }
    
    public string? Address { get; set; }
    
    public double? Longitude { get; set; }
    
    public double? Latitude { get; set; }
    
    public string? Description { get; set; }

    public int ProvinceId { get; set; }

    public Address Province { get; set; } = default!;
    
    public int DistrictId { get; set; }
    
    public Address District { get; set; } = default!;
    
    public int WardId { get; set; }

    public Address Ward { get; set; } = default!;
}