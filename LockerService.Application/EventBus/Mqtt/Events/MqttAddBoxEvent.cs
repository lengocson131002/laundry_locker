namespace LockerService.Application.EventBus.Mqtt.Events;

public class MqttAddBoxEvent : MqttBaseMessage
{
    public string LockerCode { get; set; } = default!;
    
    public int BoxNumber { get; set; }
    
    public int BoardNo { get; set; }

    public int Pin { get; set; }
    
    [JsonIgnore] 
    public override string Topic => MqttTopics.AddBoxTopic;
}