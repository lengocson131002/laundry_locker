using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum PaymentType
{
    [Description("Thanh toán đơn hàng")]
    Checkout = 0,
    
    [Description("Đặt chỗ đơn hàng")]
    Reserve = 1,
    
    [Description("Nạp tiền vào ví")]
    Deposit = 2,
    
    [Description("Hoàn trả")]
    Refund = 3,
}