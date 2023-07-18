using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Store")]
public class Store : BaseAuditableEntity
{
    [Key] public long Id { get; set; }

    public string Name { get; set; } = default!;

    public string? ContactPhone { get; set; }

    public StoreStatus Status { get; set; } = StoreStatus.Active;

    public long LocationId { get; set; }

    public Location Location { get; set; } = default!;

    public string? Image { get; set; }

    public IList<Account> Staffs { get; set; }
    
    public IList<Locker> Lockers { get; set; }
}