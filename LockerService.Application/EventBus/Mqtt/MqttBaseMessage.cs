namespace LockerService.Application.EventBus.Mqtt;

public abstract class MqttBaseMessage
{
    public MqttBaseMessage()
    {
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public MqttBaseMessage(long timestamp)
    {
        Timestamp = timestamp;
    }
    public long Timestamp { get; set; }

    [JsonIgnore]
    public abstract string Topic { get; }
}