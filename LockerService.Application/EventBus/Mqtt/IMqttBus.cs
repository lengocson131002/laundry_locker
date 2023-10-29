namespace LockerService.Application.EventBus.Mqtt;

public interface IMqttBus
{
    Task PublishAsync<T>(T message) where T : MqttBaseMessage;
}