using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LockerService.Domain.Entities;

[Table("StaffLocker")]
public class StaffLocker : BaseAuditableEntity
{
    [Key] public long Id { get; set; }

    public long StaffId { get; set; }

    public Account Staff { get; set; } = default!;

    public long LockerId { get; set; }

    public Locker Locker { get; set; } = default!;
}