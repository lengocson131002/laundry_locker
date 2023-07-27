namespace LockerService.Application.Accounts.Models;

public class AccountResponse
{
    public long Id { get; set; }

    public string Username { get; set; } = default!;

    public string PhoneNumber { get; set; } = default!;

    public Role Role { get; set; }
    
    public AccountStatus Status { get; set; }

    public string? FullName { get; set; }

    public string? Description { get; set; }
    
    public string? Avatar { get; set; }
    
    public DateTimeOffset? CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
}