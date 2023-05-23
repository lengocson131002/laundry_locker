namespace LockerService.Infrastructure.EventBus.Mqtt;

public class MqttClientServiceProvider
{
    public readonly MqttClientService MqttClientService;

    public MqttClientServiceProvider(MqttClientService service)
    {
        MqttClientService = service;
    }
    
    
}