namespace LockerService.Application.EventBus.RabbitMq.Events.Lockers;

public class LockerAddBoxEvent : RabbitMqBaseEvent
{
    public string LockerCode { get; set; } = default!;
    
    public int BoxNumber { get; set; }
    
}