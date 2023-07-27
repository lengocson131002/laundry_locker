namespace LockerService.Application.EventBus.RabbitMq.Events.Lockers;

public class LockerDisconnectedEvent
{
    public long LockerId { get; set; }
}