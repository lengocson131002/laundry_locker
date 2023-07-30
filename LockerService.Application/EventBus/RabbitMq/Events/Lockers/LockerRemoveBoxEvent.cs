namespace LockerService.Application.EventBus.RabbitMq.Events.Lockers;

public class LockerRemoveBoxEvent : RabbitMqBaseEvent
{
    public string LockerCode { get; set; } = default!;
    
    public int BoxNumber { get; set; }
    
}