using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Locker")]
public class Locker : BaseAuditableEntity
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;
    public LockerStatus Status { get; set; } = LockerStatus.Initialized;
    
    public int LocationId { get; set; }

    public Location Location { get; set; } = default!;
    
    public int? Width { get; set; }
    
    public int? Height { get; set; }
    
    public int? Depth { get; set; }
    
    public int RowCount { get; set; }
    
    public int ColumnCount { get; set; }
    
    public string? Description { get; set; }
    
    public string? Provider { get; set; }
    
    public string MacAddress { get; set; } = default!;
    
    public IList<Hardware> Hardwares { get; private set; } = new List<Hardware>();

    public IList<LockerTimeline> Timelines { get; private set; } = new List<LockerTimeline>();

    public int BoxCount => RowCount * ColumnCount;
}