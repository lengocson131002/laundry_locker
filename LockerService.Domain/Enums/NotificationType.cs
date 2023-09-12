using System.ComponentModel;

namespace LockerService.Domain.Enums;

public enum NotificationType
{
    /**
     * COMMON NOTIFICATION TYPES
     */
    [Description("OTP created")] 
    AccountOtpCreated,

    /**
     * SYSTEM NOTIFICATION TYPES
     */
    // Account
    [Description("[System] New staff account created")] 
    SystemStaffCreated,
    
    // Locker
    [Description("[System] Locker connected to the system")]
    SystemLockerConnected,
    
    [Description("[System] Locker disconnected to the system")]
    SystemLockerDisconnected,
    
    [Description("[System] Locker is going to be overloaded")]
    SystemLockerBoxWarning,
    
    [Description("[System] Locker is overloaded")]
    SystemLockerBoxOverloaded,
    
    // Order
    [Description("[System] New order created")] 
    SystemOrderCreated,
    
    [Description("[System] Order collected to the store")]
    SystemOrderCollected,
    
    [Description("[System] Order is processed")]
    SystemOrderProcessed,

    [Description("[System] Order is overtime")]
    SystemOrderOverTime,
    
    /**
     * CUSTOMER NOTIFICATION TYPES
     */
    
    // Order
    [Description("[Customer] You has a new order created")] 
    CustomerOrderCreated,
    
    [Description("[Customer] Your order is returned to the locker")] 
    CustomerOrderReturned,
    
    [Description("[Customer] Your order is canceled")] 
    CustomerOrderCanceled,

    [Description("[Customer] Your order is completed")]
    CustomerOrderCompleted,
    
    [Description("[Customer] Your Order is overtime")]
    CustomerOrderOverTime,
    
}
