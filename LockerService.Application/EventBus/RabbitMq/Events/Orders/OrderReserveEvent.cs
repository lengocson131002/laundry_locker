namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderReservedEvent : RabbitMqBaseEvent
{
    public Order Order { get; set; } = default!;
    
    public DateTimeOffset Time { get; set; }
}