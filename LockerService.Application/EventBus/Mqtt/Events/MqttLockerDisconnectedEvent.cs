namespace LockerService.Application.EventBus.Mqtt.Events;

public class MqttLockerDisconnectedEvent: MqttBaseMessage
{
    public string LockerCode { get; set; } = default!;
    
    [JsonIgnore] 
    public override string Topic => MqttTopics.DisconnectTopic;
}