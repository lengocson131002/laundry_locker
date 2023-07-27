namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderReturnedEvent : RabbitMqBaseEvent
{
    public long Id { get; set; }
    
    public OrderStatus PreviousStatus { get; set; }
    
    public OrderStatus Status { get; set; }
}