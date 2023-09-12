using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum OrderStatus
{
    [Description("Mới khởi tạo")] 
    Initialized,
    
    [Description("Đang chờ")] 
    Waiting,
    
    [Description("Đã đưa về cửa hàng")]
    Collected,

    [Description("Đang xử lý")]
    Processing,
    
    [Description("Đã xử lý")]
    Processed,
    
    [Description("Đã giao")]
    Returned,
    
    [Description("Đã thanh toán")]
    Completed,
    
    [Description("Đã hủy")]
    Canceled,
    
    [Description("Đã đặt chỗ")]
    Reserved,

    [Description("Overtime")]
    Overtime
}