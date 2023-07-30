namespace LockerService.Application.EventBus.Mqtt.Events;

public class MqttUpdateLockerInfoEvent : MqttBaseMessage
{
    public long LockerId { get; set; }

    public string LockerCode { get; set; } = default!;

    public string LockerName { get; set; } = default!;
    
    public LockerStatus LockerStatus { get; set; }

    public string ApiHost { get; set; } = default!;

    public string ApiKey { get; set; } = default!;
    
    [JsonIgnore]
    public override string Topic => string.Format(MqttTopics.UpdateInfoTopic, LockerCode);
}