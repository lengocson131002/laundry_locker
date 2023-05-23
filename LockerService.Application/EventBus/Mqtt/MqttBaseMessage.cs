namespace LockerService.Application.EventBus.Mqtt;

public abstract class MqttBaseMessage
{
    public MqttBaseMessage()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }

    public MqttBaseMessage(Guid id, DateTime createdDate)
    {
        Id = id;
        CreatedDate = createdDate;
    }
    
    public Guid Id { get; private set; }
    
    public DateTime CreatedDate { get; private set; }
    
    public abstract string Topic { get; } 
}