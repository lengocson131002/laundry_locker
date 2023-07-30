using System.Text;
using LockerService.Application.EventBus.RabbitMq;
using LockerService.Infrastructure.Common.Constants;
using LockerService.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;

namespace LockerService.Infrastructure.EventBus.Mqtt;

public class MqttClientService : IMqttClientService
{
     private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _options;
    private readonly ILogger<MqttClientService> _logger;
    private readonly MqttSettings _settings;
    private readonly IRabbitMqBus _rabbitMqBus;
    
    public MqttClientService(
        ILogger<MqttClientService> logger, 
        MqttSettings settings,
        IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        _rabbitMqBus = scope.ServiceProvider.GetRequiredService<IRabbitMqBus>();
    
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

    private async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        try
        {
            var topic = arg.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);
            
            _logger.LogInformation($"[MQTT] Receive message. Topic: {topic}, Payload: {payload}");
            
            var jsonOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            
            switch (topic)
            {
                case MqttTopics.ConnectTopic:
                    await HandlerLockerConnected(JsonSerializer.Deserialize<MqttLockerConnectedEvent>(payload, jsonOptions));
                    break;
            
                case MqttTopics.DisconnectTopic:
                    await HandleLockerDisconnected(JsonSerializer.Deserialize<MqttLockerDisconnectedEvent>(payload, jsonOptions));
                    break;
                
                case MqttTopics.ResetBoxesTopic:
                    await HandleResetBoxes(JsonSerializer.Deserialize<MqttResetBoxesEvent>(payload, jsonOptions));
                    break;
                
                case MqttTopics.AddBoxTopic:
                    await HandlerAddBox(JsonSerializer.Deserialize<MqttAddBoxEvent>(payload, jsonOptions));
                    break;
                
                case MqttTopics.RemoveBoxTopic:
                    await HandleRemoveBox(JsonSerializer.Deserialize<MqttRemoveBoxEvent>(payload, jsonOptions));
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error when handle mqtt message: {0}", ex.Message);
        }

    }

    private async Task HandleRemoveBox(MqttRemoveBoxEvent? @event)
    {
        if (@event == null)
        {
            return;
        }
        
        await _rabbitMqBus.PublishAsync(new LockerRemoveBoxEvent()
        {
            LockerCode = @event.LockerCode,
            BoxNumber = @event.BoxNumber
        });
    }

    private async Task HandlerAddBox(MqttAddBoxEvent? @event)
    {
        if (@event == null)
        {
            return;
        }

        await _rabbitMqBus.PublishAsync(new LockerAddBoxEvent()
        {
            LockerCode = @event.LockerCode,
            BoxNumber = @event.BoxNumber
        });
    }

    private async Task HandleResetBoxes(MqttResetBoxesEvent? @event)
    {
        if (@event == null)
        {
            return;
        }
        
        await _rabbitMqBus.PublishAsync(new LockerResetBoxesEvent()
        {
            LockerCode = @event.LockerCode
        });
    }

    private async Task HandleLockerDisconnected(MqttLockerDisconnectedEvent? @event)
    {
        if (@event == null)
        {
            return;
        }

        await _rabbitMqBus.PublishAsync(new LockerDisconnectedEvent()
        {
            LockerCode = @event.LockerCode
        });
    }

    private async Task HandlerLockerConnected(MqttLockerConnectedEvent? @event)
    {
        if (@event == null)
        {
            return;
        }

        await _rabbitMqBus.PublishAsync(new LockerConnectedEvent()
        {
            LockerCode = @event.LockerCode,
            MacAddress = @event.MacAddress,
            IpAddress = @event.IpAddress
        });
    }

    private async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        _logger.LogInformation("MQTT Client Connected");
        await Task.CompletedTask;
    }

    private async Task HandleConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        _logger.LogInformation("MQTT Client Connected");
        await Task.WhenAll(
            _mqttClient.SubscribeAsync(MqttTopics.ConnectTopic),
            _mqttClient.SubscribeAsync(MqttTopics.DisconnectTopic),
            _mqttClient.SubscribeAsync(MqttTopics.ResetBoxesTopic),
            _mqttClient.SubscribeAsync(MqttTopics.AddBoxTopic),
            _mqttClient.SubscribeAsync(MqttTopics.RemoveBoxTopic));
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
                        if (!await _mqttClient.TryPingAsync(cancellationToken))
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