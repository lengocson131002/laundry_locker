using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("AccountLocker")]
public class AccountLocker : BaseAuditableEntity
{
    [Key] public int Id { get; set; }
    public int AccountId { get; set; }
    public Account Account { get; set; }
    public int LockerId { get; set; }
    public Locker Locker { get; set; }
}