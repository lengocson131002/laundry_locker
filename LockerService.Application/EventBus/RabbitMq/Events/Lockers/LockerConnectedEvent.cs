namespace LockerService.Application.EventBus.RabbitMq.Events.Lockers;

public class LockerConnectedEvent : RabbitMqBaseEvent
{
    public long LockerId { get; set; }
}