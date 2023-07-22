namespace LockerService.Application.EventBus.RabbitMq.Events.Lockers;

public class LockerConnectedEvent
{
    public long LockerId { get; set; }
}