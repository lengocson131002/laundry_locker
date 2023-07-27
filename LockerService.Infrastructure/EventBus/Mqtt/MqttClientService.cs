using LockerService.Infrastructure.Settings;
using MQTTnet;
using MQTTnet.Client;

namespace LockerService.Infrastructure.EventBus.Mqtt;

public class MqttClientService : IMqttClientService
{
     private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _options;
    private readonly ILogger<MqttClientService> _logger;
    private readonly MqttSettings _settings;
    
    public MqttClientService(ILogger<MqttClientService> logger, MqttSettings settings)
    {
        _settings = settings;
        _mqttClient = new MqttFactory().CreateMqttClient();
        _options = new MqttClientOptionsBuilder()
            .WithTcpServer(_settings.Host, _settings.Port)
            .WithClientId(Guid.NewGuid().ToString())
            .WithCredentials(_settings.Username, _settings.Password)
            .WithCleanSession()
            .Build();
        
        _logger = logger;
        ConfigureMqttClient();
    }

    private void ConfigureMqttClient()
    {
        _mqttClient.ConnectedAsync += HandleConnectedAsync;
        _mqttClient.DisconnectedAsync += HandleDisconnectedAsync;
        _mqttClient.ApplicationMessageReceivedAsync += HandleApplicationMessageReceivedAsync;
    }

    private Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        throw new NotImplementedException();
    }

    private async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        _logger.LogInformation("MQTT Client Connected");
        await Task.CompletedTask;
    }

    private async Task HandleConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        _logger.LogInformation("MQTT Client Connected");
        await _mqttClient.SubscribeAsync("Aqswde123@");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Connect to MQTT Client
        await _mqttClient.ConnectAsync(_options, cancellationToken);
        
        _ = Task.Run(
            async () =>
            {
                while (true)
                {
                    try
                    {
                        if (!await _mqttClient.TryPingAsync())
                        {
                            await _mqttClient.ConnectAsync(_options, CancellationToken.None);
                            _logger.LogInformation("The MQTT client is reconnected.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "The MQTT client connection failed");
                    }
                    finally
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), CancellationToken.None);
                    }
                }
            }, CancellationToken.None);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            var disconnectOption = new MqttClientDisconnectOptions
            {
                Reason = MqttClientDisconnectOptionsReason.NormalDisconnection,
                ReasonString = "NormalDisconnection"
            };
            await _mqttClient.DisconnectAsync(disconnectOption, cancellationToken);
        }
        await _mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
    }

    public IMqttClient MqttClient => _mqttClient;

    public string? MqttSecretKey => _settings.SecretKey;
    
    public bool IsEncrypted => _settings.IsEncrypted;
}