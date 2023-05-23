using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Account")]
public class Account : BaseAuditableEntity
{
    public long Id { get; set; }

    public string? Email { get; set; }
    
    public string PhoneNumber { get; set; } = default!;
    
    public string? Password { get; set; }

    public Role Role { get; set; }

    public string? FullName { get; set; }
    
    public string? Description { get; set; }
}