namespace LockerService.Application.EventBus.RabbitMq.Events.Lockers;

public class LockerResetBoxesEvent : RabbitMqBaseEvent
{
    public string LockerCode { get; set; } = default!;
}