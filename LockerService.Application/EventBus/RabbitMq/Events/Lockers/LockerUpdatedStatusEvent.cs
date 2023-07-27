namespace LockerService.Application.EventBus.RabbitMq.Events.Lockers;

public class LockerUpdatedStatusEvent : RabbitMqBaseEvent
{
    public long LockerId { get; set; }
    
    public LockerStatus Status { get; set; }
    
    public LockerStatus PreviousStatus { get; set; }
}