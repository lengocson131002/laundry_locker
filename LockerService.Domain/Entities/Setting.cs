using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerService.Domain.Entities;

[Table("Setting")]
public class Setting : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    public string Key { get; set; } = default!;

    [Column(TypeName = "jsonb")]
    public string Value { get; set; } = default!;

}