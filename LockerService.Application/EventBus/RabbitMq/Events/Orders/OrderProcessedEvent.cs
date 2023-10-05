namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderProcessedEvent : RabbitMqBaseEvent
{
    public Order Order { get; set; } = default!;
    
    public DateTimeOffset Time { get; set; }

    public Account Staff { get; set; } = default!;
    
    public OrderStatus PreviousStatus { get; set; }
}