namespace LockerService.Application.EventBus.RabbitMq.Events.Lockers;

public class LockerConnectedEvent : RabbitMqBaseEvent
{
    public string LockerCode { get; set; } = default!;

    public string? MacAddress { get; set; }
    
    public string? IpAddress { get; set; }
}