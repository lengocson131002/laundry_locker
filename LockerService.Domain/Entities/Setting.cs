using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Setting")]
public class Setting : BaseAuditableEntity
{
    [Key]
    public long Id { get; set; }
    
    public SettingGroup Group { get; set; }

    public string Key { get; set; } = default!;

    public string Value { get; set; } = default!;

    public string Name { get; set; } = default!;

}