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
            Password = "$2a$12$yNH0Ks5IgyhzQVj96w7/qOCVW8JPQQYM5oKIB/1nwA6xOFYTkterq"
        }
    };
}