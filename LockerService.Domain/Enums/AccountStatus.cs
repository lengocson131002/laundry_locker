using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum AccountStatus
{
    [Description("Đang hoạt động")]
    Active,
    
    [Description("Ngưng hoạt động")]
    Inactive,
    
    [Description("Mới khởi tạo")]
    Verifying
}