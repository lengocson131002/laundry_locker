using LockerService.Application.Common.Security;
using MQTTnet;
using MQTTnet.Protocol;

namespace LockerService.Infrastructure.EventBus.Mqtt;

public class MqttBus : IMqttBus
{
    private readonly ISecurityService _securityService;
    private readonly ILogger<MqttBus> _logger;
    private readonly MqttClientService _mqttClientService;

    public MqttBus(
        ISecurityService securityService, 
        ILogger<MqttBus> logger,
        MqttClientServiceProvider mqttClientServiceProvider)
    {
        _securityService = securityService;
        _logger = logger;
        _mqttClientService = mqttClientServiceProvider.MqttClientService;
    }

    public async Task PublishAsync<T>(T message) where T : MqttBaseMessage
    {
        var messagePayload = _mqttClientService.IsEncrypted && _mqttClientService.MqttSecretKey != null
            ? await _securityService.EncryptToBase64Async(JsonSerializer.Serialize(message),
                _mqttClientService.MqttSecretKey)
            : JsonSerializer.Serialize(message);

        var mqttClient = _mqttClientService.MqttClient;
        if (mqttClient.IsConnected)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(message.Topic)
                .WithPayload(messagePayload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();
            await mqttClient.PublishAsync(applicationMessage);
            
            _logger.LogInformation("Push MQTT Message {message}", messagePayload);
        }
        else
        {
            _logger.LogInformation("MQTT client disconnected");
        }
    }
}