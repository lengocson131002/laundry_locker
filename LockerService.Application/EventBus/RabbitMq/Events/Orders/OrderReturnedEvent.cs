namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderReturnedEvent : RabbitMqBaseEvent
{
    public int Id { get; set; }
}