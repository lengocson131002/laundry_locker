using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum AccountStatus
{
    [Description("Đang hoạt động")]
    Active = 0,
    
    [Description("Ngưng hoạt động")]
    Inactive = 1,
    
    [Description("Mới khởi tạo")]
    Verifying = 2
}