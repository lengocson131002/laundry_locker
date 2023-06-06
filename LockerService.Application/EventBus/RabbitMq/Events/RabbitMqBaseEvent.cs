namespace LockerService.Application.EventBus.RabbitMq.Events;

public class RabbitMqBaseEvent
{
    public RabbitMqBaseEvent()
    {
        EventId = Guid.NewGuid();
        IssuedDate = DateTime.UtcNow;
    }

    public RabbitMqBaseEvent(Guid id, DateTime createdDate)
    {
        EventId = id;
        IssuedDate = createdDate;
    }
    
    public Guid EventId { get; private set; }
    
    public DateTimeOffset IssuedDate { get; private set; }
}