namespace LockerService.Application.EventBus.Mqtt.Events;

public class MqttOpenBoxEvent : MqttBaseMessage
{
    public string LockerCode { get; set; } = default!;
    
    public int BoxNumber { get; set; }

    [JsonIgnore]
    public override string Topic => string.Format(MqttTopics.OpenBoxTopic, LockerCode);

}