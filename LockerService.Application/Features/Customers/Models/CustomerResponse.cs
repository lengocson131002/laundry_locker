using LockerService.Application.Features.Accounts.Models;
using LockerService.Application.Features.Wallets.Models;

namespace LockerService.Application.Features.Customers.Models;

public class CustomerResponse : AccountResponse
{
    public WalletResponse? Wallet { get; set; }
}