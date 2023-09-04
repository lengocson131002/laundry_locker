namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderInitializedEvent : RabbitMqBaseEvent
{
    public long OrderId { get; set; }
    
    public DateTimeOffset Time { get; set; }
    
    public OrderStatus Status { get; set; }
    
    public OrderStatus? PreviousStatus { get; set; }
    
}