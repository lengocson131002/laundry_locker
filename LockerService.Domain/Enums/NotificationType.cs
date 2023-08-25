using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum NotificationType
{
    // Order
    [Description("New order created")] 
    OrderCreated,
    
    [Description("Order is returned to the locker")] 
    OrderReturned,
    
    [Description("Order is canceled")] 
    OrderCanceled,

    [Description("Order is completed")]
    OrderCompleted,
    
    // Account
    [Description("New staff account created")] 
    AccountStaffCreated,
    
    [Description("OTP created")] 
    AccountOtpCreated,
}
