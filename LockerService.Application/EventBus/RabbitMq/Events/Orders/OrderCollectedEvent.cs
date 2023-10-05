namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderCollectedEvent : RabbitMqBaseEvent
{
    public Order Order { get; set; } = default!;
    
    public DateTimeOffset Time { get; set; }
    
    public OrderStatus PreviousStatus { get; set; }

    public Account Staff { get; set; } = default!;
}