using LockerService.Domain.Enums;
using LockerService.Infrastructure.Settings;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerConnectedConsumer : IConsumer<LockerConnectedEvent>
{
    private readonly ILogger<LockerConnectedConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly ApiKeySettings _apiKeySettings;
    private readonly IServer _server;

    public LockerConnectedConsumer(
        ILogger<LockerConnectedConsumer> logger, 
        IUnitOfWork unitOfWork, 
        IMqttBus mqttBus, 
        ApiKeySettings apiKeySettings, 
        IServer server)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _apiKeySettings = apiKeySettings;
        _server = server;
    }

    public async Task Consume(ConsumeContext<LockerConnectedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received locker connected message: {0}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository
            .Get(locker => locker.Code == message.LockerCode)
            .FirstOrDefaultAsync();
        
        if (locker == null)
        {
            return;
        }

        locker.Status = LockerStatus.Active;
        locker.IpAddress = message.IpAddress;
        locker.MacAddress = message.MacAddress;
        
        await _unitOfWork.LockerRepository.UpdateAsync(locker);
        
        var @event = new LockerTimeline()
        {
            LockerId = locker.Id,
            Event = LockerEvent.Connect,
            Data = JsonSerializer.Serialize(locker),
            Status = locker.Status
        };
        
        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();
        
        // Send info back to locker
        var features = _server.Features;
        var addresses = features.Get<IServerAddressesFeature>();
        var host = addresses?.Addresses.FirstOrDefault();
        await _mqttBus.PublishAsync(new MqttUpdateLockerInfoEvent()
        {
            LockerId = locker.Id,
            LockerCode = locker.Code,
            LockerName = locker.Name,
            LockerStatus = locker.Status,
            ApiHost = host,
            ApiKey = _apiKeySettings.Key,
        });
    }
}