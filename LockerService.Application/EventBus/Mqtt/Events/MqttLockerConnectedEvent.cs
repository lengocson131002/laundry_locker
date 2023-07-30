namespace LockerService.Application.EventBus.Mqtt.Events;

public class MqttLockerConnectedEvent : MqttBaseMessage
{
    public string LockerCode { get; set; } = default!;

    public string? IpAddress { get; set; }

    public string? MacAddress { get; set; }

    [JsonIgnore] 
    public override string Topic => MqttTopics.ConnectTopic;
}