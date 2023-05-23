using LockerService.Domain.Entities;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.Data;

public static class AccountSeeding
{
    public static IList<Account> DefaultAccounts => new List<Account>()
    {
        new()
        {
            Email = "admin@gmail.com",
            PhoneNumber = "0367537978",
            Password = "Aqswde123@",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            Role = Role.Admin
        }
    };
}