namespace LockerService.Application.EventBus.Mqtt.Events;

public class MqttUpdateLockerStatusEvent : MqttBaseMessage
{
    public long LockerId { get; private set; }
 
    public string Status { get; private set; }

    public MqttUpdateLockerStatusEvent(long lockerId, LockerStatus status) : base()
    {
        LockerId = lockerId;
        Status = status.ToString();
        Topic = $"locker/{LockerId}/status/update";
    }
    
    public override string Topic { get; }
}