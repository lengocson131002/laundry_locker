using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.Projectables;
using System.Text.Json.Serialization;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Account")]
public class Account : BaseAuditableEntity
{
    [Key] 
    public long Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonIgnore] 
    public string Password { get; set; } = string.Empty;

    public Role Role { get; set; }
    
    public AccountStatus Status { get; set; } = AccountStatus.Active;

    public string? FullName { get; set; }

    [Column(TypeName = "text")]
    public string? Description { get; set; }
    
    public string? Avatar { get; set; }

    public long? StoreId { get; set; }
    
    public Store? Store { get; set; }
    
    [Projectable]
    public bool IsActive => Equals(AccountStatus.Active, Status);

    [Projectable]
    public bool IsAdmin => Equals(Role, Role.Admin);

    [Projectable]
    public bool IsManager => Equals(Role, Role.Manager);

    [Projectable]
    public bool IsLaundryAttendant => Equals(Role, Role.LaundryAttendant);

    [Projectable]
    public bool IsCustomer => Equals(Role, Role.Customer);

    [Projectable] 
    public bool IsStaff => IsManager || IsLaundryAttendant || IsAdmin;

    [Projectable]
    public bool IsStoreStaff => StoreId != null && (IsManager || IsLaundryAttendant);

    public bool CanUpdateStatus(AccountStatus status)
    {
        if (Equals(Status, status))
        {
            return true;
        }
        switch (status)
        {
            case AccountStatus.Active:
            case AccountStatus.Inactive:
                return !Equals(Status, AccountStatus.Verifying);
            
            default:
                return false;
        }
    }
}