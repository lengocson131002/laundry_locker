namespace LockerService.Application.EventBus.RabbitMq.Events.Lockers;

public class LockerOverloadedEvent : RabbitMqBaseEvent
{
    public long LockerId { get; set; }
    
    public DateTimeOffset Time { get; set; }
    
    public int ErrorCode { get; set; }

    public string Error { get; set; } = default!;
}