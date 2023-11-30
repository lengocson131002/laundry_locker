namespace LockerService.Application.Features.Wallets.Models;

public class WalletResponse
{
    public long Id { get; set; }
    
    public decimal Balance { get; set; }
    
    public DateTimeOffset? LastDepositAt { get; set; }
}