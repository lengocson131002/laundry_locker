using LockerService.Domain.Enums;
using LockerService.Infrastructure.Settings;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerUpdatedInfoConsumer : IConsumer<LockerUpdatedInfoEvent>
{
    private readonly ILogger<LockerUpdatedInfoConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly ApiKeySettings _apiKeySettings;
    private readonly ServerSettings _serverSettings;

    public LockerUpdatedInfoConsumer(
        ILogger<LockerUpdatedInfoConsumer> logger, 
        IUnitOfWork unitOfWork, 
        IMqttBus mqttBus, 
        ApiKeySettings apiKeySettings, 
        ServerSettings serverSettings)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _apiKeySettings = apiKeySettings;
        _serverSettings = serverSettings;
    }

    public async Task Consume(ConsumeContext<LockerUpdatedInfoEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received locker updated message: {0}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(message.Id);
        if (locker == null)
        {
            return;
        }

        var @event = new LockerTimeline()
        {
            LockerId = locker.Id,
            Event = LockerEvent.UpdateInformation,
            Status = locker.Status,
            Data = message.Data
        };

        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();
        
        // Push MQTT Event to update locker info
        await _mqttBus.PublishAsync(new MqttUpdateLockerInfoEvent()
        {
            LockerId = locker.Id,
            LockerCode = locker.Code,
            LockerName = locker.Name,
            LockerStatus = locker.Status,
            ApiHost = _serverSettings.Host,
            ApiKey = _apiKeySettings.Key,
        });
    }
}