using LockerService.Domain.Entities;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.Data;

public static class AccountSeeding
{
    public static IList<Account> DefaultAccounts => new List<Account>()
    {
        new()
        {
            Username = "admin",
            PhoneNumber = "0367537978",
            Status = AccountStatus.Active,
            Role = Role.Admin,
            Password = "Aqswde123@"
        }
    };
}