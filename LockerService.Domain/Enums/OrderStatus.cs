using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum OrderStatus
{
    [Description("Mới khởi tạo")] 
    Initialized = 0,
    
    [Description("Đang chờ")] 
    Waiting = 1,
    
    [Description("Đã đưa về cửa hàng")]
    Collected = 2,

    [Description("Đang xử lý")]
    Processing = 3,
    
    [Description("Đã xử lý")]
    Processed = 4,
    
    [Description("Đã giao")]
    Returned = 5,
    
    [Description("Đã thanh toán")]
    Completed = 6,
    
    [Description("Đã hủy")]
    Canceled = 7,
    
    [Description("Đã đặt chỗ")]
    Reserved = 8,

    [Description("Quá hạn")]
    Overtime = 9,
    
    [Description("Đang bỏ thêm đồ")]
    Updating = 10,
    
    [Description("Đang xử lý quá hạn")]
    OvertimeProcessing = 11
    
    
}