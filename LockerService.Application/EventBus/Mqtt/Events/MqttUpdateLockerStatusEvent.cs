using LockerService.Domain.Enums;

namespace LockerService.Application.EventBus.Mqtt.Events;

public class MqttUpdateLockerStatusEvent : MqttBaseMessage
{
    public int LockerId { get; private set; }
 
    public string Status { get; private set; }

    public MqttUpdateLockerStatusEvent(int lockerId, LockerStatus status) : base()
    {
        LockerId = lockerId;
        Status = status.ToString();
        Topic = $"locker/{LockerId}/status/update";
    }
    
    public override string Topic { get; }
}