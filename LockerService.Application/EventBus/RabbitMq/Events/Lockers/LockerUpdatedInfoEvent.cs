namespace LockerService.Application.EventBus.RabbitMq.Events.Lockers;

public class LockerUpdatedInfoEvent : RabbitMqBaseEvent
{
    public long Id { get; set; }
    
    public DateTimeOffset Time { get; set; }

    public string Data { get; set; } = default!;
    
}