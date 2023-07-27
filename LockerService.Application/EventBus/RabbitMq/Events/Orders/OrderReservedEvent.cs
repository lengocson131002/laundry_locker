namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderReservedEvent : RabbitMqBaseEvent
{
    public long OrderId { get; set; }
    
    public OrderStatus Status { get; set; }
}