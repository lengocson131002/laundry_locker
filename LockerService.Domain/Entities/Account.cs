using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Account")]
public class Account : BaseAuditableEntity
{
    [Key] 
    public long Id { get; set; }
    
    public string Username { get; set; } = default!;
    
    public string PhoneNumber { get; set; } = default!;

    public string? Password { get; set; }

    public Role Role { get; set; }
    
    public AccountStatus Status { get; set; } = AccountStatus.Active;

    public string? FullName { get; set; }

    [Column(TypeName = "text")]
    public string? Description { get; set; }
    
    public string? Avatar { get; set; }

    public long? StoreId { get; set; }
    
    public Store? Store { get; set; }
    
    public IList<StaffLocker> StaffLockers { get; private set; } = new List<StaffLocker>();
}