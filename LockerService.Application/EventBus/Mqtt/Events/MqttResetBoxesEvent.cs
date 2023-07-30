namespace LockerService.Application.EventBus.Mqtt.Events;

public class MqttResetBoxesEvent : MqttBaseMessage
{
    public string LockerCode { get; set; } = default!;
    
    [JsonIgnore] 
    public override string Topic => MqttTopics.ResetBoxesTopic;
}