namespace LockerService.Application.EventBus.RabbitMq.Events;

public class OrderReturnedEvent : RabbitMqBaseEvent
{
    public int Id { get; set; }
}