using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerService.Domain.Entities;

[Table("Location")]
public class Location : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public string? Address { get; set; }
    
    public double? Longitude { get; set; }
    
    public double? Latitude { get; set; }
    
    [Column(TypeName = "text")]
    public string? Description { get; set; }

    public long ProvinceId { get; set; }

    public Address Province { get; set; } = default!;
    
    public long DistrictId { get; set; }
    
    public Address District { get; set; } = default!;
    
    public long WardId { get; set; }

    public Address Ward { get; set; } = default!;

    public override string ToString()
    {
        return $"{Address}, {Ward}, {District}, {Province}";
    }
}