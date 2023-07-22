namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderCreatedEvent : RabbitMqBaseEvent
{
    public int Id { get; set; }
    
    public string? PinCode { get; set; }
    
    public DateTimeOffset? PinCodeIssuedAt { get; set; } 
    
    public int SendBoxOrder { get; set; }

    public string OrderPhone { get; set; } = default!;
    
    public string? ReceivePhone { get; set; }
    
    public int ReceiveBoxOrder { get; set; }
    
    public DateTimeOffset? ReceiveTime { get; set; }
    
    public DateTimeOffset? ActualReceiveTime { get; set; }
    
    public OrderStatus Status { get; set; } = OrderStatus.Initialized;
    
    public int ServiceId { get; set; }

    public Service Service { get; set; } = default!;
    
    public int LockerId { get; set; }
}