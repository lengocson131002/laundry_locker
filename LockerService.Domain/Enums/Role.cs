using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum Role
{
    [Description("Quản trị viên")]
    Admin, 
    
    [Description("Nhân viên giặt sấy")]
    LaundryAttendant,
    
    [Description("Quản lý cửa hàng")]
    Manager,
    
    [Description("Nhân viên nhận và trả đồ")]
    Shipper,

    [Description("Khách hàng")]
    Customer
}