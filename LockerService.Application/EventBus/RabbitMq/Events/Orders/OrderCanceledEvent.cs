namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderCanceledEvent : RabbitMqBaseEvent
{
    public Order Order { get; set; } = default!;
    
    public DateTimeOffset Time { get; set; }
    
    public OrderStatus PreviousStatus { get; set; }
    
}