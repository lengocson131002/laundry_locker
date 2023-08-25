using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum OrderStatus
{
    [Description("Mới khởi tạo")] 
    Initialized,
    
    [Description("Đang chờ")] 
    Waiting,
    
    [Description("Đang xử lý")]
    Processing,
    
    [Description("Đã xử lý")]
    Returned,
    
    [Description("Đã hoàn thành")]
    Completed,
    
    [Description("Đã hủy")]
    Canceled,
    
    [Description("Đã đặt chỗ")]
    Reserved,
}