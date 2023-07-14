namespace LockerService.Application.Accounts.Models;

public class AccountResponse
{
    public int Id { get; set; }

    public string Username { get; set; }
    public string PhoneNumber { get; set; }

    public string? Password { get; set; }

    public Role Role { get; set; }
    public AccountStatus Status { get; set; }

    public string? FullName { get; set; }

    public string? Description { get; set; }
    public string? Avatar { get; set; }
    public int? StoreId { get; set; }
}