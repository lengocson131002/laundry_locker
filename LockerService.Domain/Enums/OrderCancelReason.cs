using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum OrderCancelReason
{
    [Description("Hết thời gian chờ")] 
    Timeout = 0,
    
    [Description("Khách hàng hủy")] 
    CustomerCancel = 1,
    
    [Description("Nhân viên hủy")] 
    StaffCancel = 2,
    
    [Description("Hủy cọc")] 
    ReservationCancel = 3
}