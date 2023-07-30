namespace LockerService.Application.EventBus.Mqtt;

public static class MqttTopics
{
    public const string ConnectTopic = "lockers/connect";

    public const string DisconnectTopic = "lockers/disconnect";

    public const string ResetBoxesTopic = "lockers/reset-boxes";

    public const string AddBoxTopic = "lockers/add-box";
    
    public const string RemoveBoxTopic = "lockers/remove-box";

    public const string OpenBoxTopic = "lockers/open-box/{0}";

    public const string UpdateInfoTopic = "lockers/update-info/{0}";
}