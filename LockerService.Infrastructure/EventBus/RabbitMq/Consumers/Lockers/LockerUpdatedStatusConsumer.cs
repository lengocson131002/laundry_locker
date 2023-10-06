using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Utils;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Settings;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerUpdatedStatusConsumer : IConsumer<LockerUpdatedStatusEvent>
{
    private readonly ILogger<LockerUpdatedStatusConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly ApiKeySettings _apiKeySettings;
    private readonly ServerSettings _serverSettings;

    public LockerUpdatedStatusConsumer(ILogger<LockerUpdatedStatusConsumer> logger, 
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

    public async Task Consume(ConsumeContext<LockerUpdatedStatusEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received locker updated status message: {0}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(message.LockerId);
        var lockerInfoData = JsonSerializer.Serialize(locker, JsonSerializerUtils.GetGlobalJsonSerializerOptions());
        if (locker == null)
        {
            return;
        }
        
        var @event = new LockerTimeline()
        {
            LockerId = locker.Id,
            Event = LockerEvent.UpdateStatus,
            Status = message.Status,
            PreviousStatus = message.PreviousStatus,
            Data = lockerInfoData
        };

        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();
        
        // Push MQTT event
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