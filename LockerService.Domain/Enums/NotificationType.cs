using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum NotificationType
{
    /**
     * COMMON NOTIFICATION TYPES
     */
    [Description("OTP created")] 
    AccountOtpCreated = 0,

    /**
     * SYSTEM NOTIFICATION TYPES
     */
    // Account
    [Description("[System] New staff account created")] 
    SystemStaffCreated = 1,
    
    // Locker
    [Description("[System] Locker connected to the system")]
    SystemLockerConnected = 2,
    
    [Description("[System] Locker disconnected to the system")]
    SystemLockerDisconnected = 3,
    
    [Description("[System] Locker is going to be overloaded")]
    SystemLockerBoxWarning = 4,
    
    [Description("[System] Locker is overloaded")]
    SystemLockerBoxOverloaded = 5,
    
    // Order
    [Description("[System] New order created")] 
    SystemOrderCreated = 6,

    [Description("[System] Order is overtime")]
    SystemOrderOverTime = 7,
    
    /**
     * CUSTOMER NOTIFICATION TYPES
     */
    
    // Order
    [Description("[Customer] You has a new order created")] 
    CustomerOrderCreated = 8,
    
    [Description("[Customer] Your order is returned to the locker")] 
    CustomerOrderReturned = 9,
    
    [Description("[Customer] Your order is canceled")] 
    CustomerOrderCanceled = 10,

    [Description("[Customer] Your order is completed")]
    CustomerOrderCompleted = 11,
    
    [Description("[Customer] Your Order is overtime")]
    CustomerOrderOverTime = 12,
    
    [Description("[Customer] Your order is overtime proccessed")]
    CustomerOrderOverTimeProcessing = 13,
    
    [Description("[Customer] You deposited successfully")]
    CustomerDepositCompleted = 14
 
}

