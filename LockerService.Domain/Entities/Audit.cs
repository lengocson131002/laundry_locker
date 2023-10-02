using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerService.Domain.Entities;

[Table("Audit")]
public class Audit : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public string Type { get; set; } = default!;

    public string TableName { get; set; } = default!;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? AffectedColumns { get; set; }

    public string PrimaryKey { get; set; } = default!;
}