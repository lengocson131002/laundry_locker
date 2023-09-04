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
    
    [Description("Order is overtime")]
    OrderOverTime,
    
    // Account
    [Description("New staff account created")] 
    AccountStaffCreated,
    
    [Description("OTP created")] 
    AccountOtpCreated,

    // Locker
    [Description("Locker connected to the system")]
    LockerConnected,
    
    [Description("Locker disconnected to the system")]
    LockerDisconnected,
    
    [Description("Locker is going to be overloaded")]
    LockerBoxWarning,
    
    [Description("Locker is overloaded")]
    LockerBoxOverloaded
}
