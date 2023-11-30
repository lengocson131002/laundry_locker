using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockerService.Domain.Entities;

[Table("Wallet")]
public class Wallet : BaseAuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public decimal Balance { get; set; }
    
    public long CustomerId { get; set; }

    public Account Customer { get; set; } = default!;
    
    public DateTimeOffset? LastDepositAt { get; set; }

    public Wallet()
    {
        Balance = 0;
    }

    public Wallet(decimal initialBalance)
    {
        Balance = initialBalance;
    }
}