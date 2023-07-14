using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("StaffLocker")]
public class AccountLocker : BaseAuditableEntity
{
    [Key] 
    public int Id { get; set; }
    
    public int StaffId { get; set; }
    
    public Account Staff { get; set; } = default!;
    
    public int LockerId { get; set; }
    
    public Locker Locker { get; set; } = default!;
}