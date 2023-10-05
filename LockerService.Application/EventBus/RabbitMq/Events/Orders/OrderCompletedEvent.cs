namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderCompletedEvent : RabbitMqBaseEvent
{
    public Order Order { get; set; } = default!;
    
    public OrderStatus PreviousStatus { get; set; }
    
    public DateTimeOffset Time { get; set; }

}