namespace LockerService.Application.EventBus.RabbitMq.Events.Lockers;

public class LockerDisconnectedEvent : RabbitMqBaseEvent
{
    public string LockerCode { get; set; } = default!;
}