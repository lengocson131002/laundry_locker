namespace LockerService.Application.EventBus.Mqtt.Events;

public class MqttRemoveBoxEvent : MqttBaseMessage
{
    public string LockerCode { get; set; } = default!;
    
    public int BoxNumber { get; set; }
    
    [JsonIgnore] 
    public override string Topic => MqttTopics.RemoveBoxTopic;
}