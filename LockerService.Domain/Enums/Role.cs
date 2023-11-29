using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum Role
{
    [Description("Quản trị viên")]
    Admin = 0, 
    
    [Description("Nhân viên giặt sấy")]
    LaundryAttendant = 1,
    
    [Description("Quản lý cửa hàng")]
    Manager = 2,
    
    [Description("Khách hàng")]
    Customer = 3
}