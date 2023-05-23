namespace LockerService.Application.EventBus.Mqtt.Events;

public class MqttOpenBoxEvent : MqttBaseMessage 
{
    public int LockerId { get; private set; }
    
    public int BoxOrder { get; private set; }

    public MqttOpenBoxEvent(int lockerId, int boxOrder) : base()
    {
        this.LockerId = lockerId;
        this.BoxOrder = boxOrder;
        this.Topic = $"locker/{LockerId}/box/open";
    }

    public override string Topic { get; }
}