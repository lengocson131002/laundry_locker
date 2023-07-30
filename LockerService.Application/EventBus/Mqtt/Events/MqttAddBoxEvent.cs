namespace LockerService.Application.EventBus.Mqtt.Events;

public class MqttAddBoxEvent : MqttBaseMessage
{
    public string LockerCode { get; set; } = default!;
    
    public int BoxNumber { get; set; }
    
    [JsonIgnore] 
    public override string Topic => MqttTopics.AddBoxTopic;
}