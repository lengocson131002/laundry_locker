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

    public IList<MqttBoxInformation> Boxes { get; set; } = new List<MqttBoxInformation>();
}

public class MqttBoxInformation
{
    public int Number { get; set; }
    
    public bool IsActive { get; set; }
    
    public int? BoardNo { get; set; }
    
    public int? Pin { get; set; }
}