using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Settings;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerConnectedConsumer : IConsumer<LockerConnectedEvent>
{
    private readonly ILogger<LockerConnectedConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly ApiKeySettings _apiKeySettings;
    private readonly ServerSettings _serverSettings;
    private readonly INotifier _notifier;
    public LockerConnectedConsumer(
        ILogger<LockerConnectedConsumer> logger, 
        IUnitOfWork unitOfWork, 
        IMqttBus mqttBus, 
        ApiKeySettings apiKeySettings, 
        ServerSettings serverSettings, INotifier notifier)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _apiKeySettings = apiKeySettings;
        _serverSettings = serverSettings;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<LockerConnectedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received locker connected message: {0}", JsonSerializer.Serialize(message));
        
        var locker = await _unitOfWork.LockerRepository
            .Get(
                predicate: locker => locker.Code == message.LockerCode,
                includes: new List<Expression<Func<Locker, object>>>()
                {
                    locker => locker.Location,
                    locker => locker.Location.Province,
                    locker => locker.Location.District,
                    locker => locker.Location.Ward,
                    locker => locker.Store,
                })
            .FirstOrDefaultAsync();
        
        if (locker == null)
        {
            return;
        }

        // Add event
        var lockerInfoData = JsonSerializer.Serialize(locker, JsonSerializerUtils.GetGlobalJsonSerializerOptions());
        var @event = new LockerTimeline()
        {
            LockerId = locker.Id,
            Event = LockerEvent.Connect,
            Data = lockerInfoData,
            Status = LockerStatus.Active,
            PreviousStatus = locker.Status
        };
        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);
        
        // Update locker
        locker.Status = LockerStatus.Active;
        locker.IpAddress = message.IpAddress;
        locker.MacAddress = message.MacAddress;
        await _unitOfWork.LockerRepository.UpdateAsync(locker);

        await _unitOfWork.SaveChangesAsync();
        
        // Send info back to locker
        await _mqttBus.PublishAsync(new MqttUpdateLockerInfoEvent()
        {
            LockerId = locker.Id,
            LockerCode = locker.Code,
            LockerName = locker.Name,
            LockerStatus = locker.Status,
            ApiHost = _serverSettings.Host,
            ApiKey = _apiKeySettings.Key,
        });
        
        // Push notification admins and store's staff
        var staffs = await _unitOfWork.AccountRepository
            .GetStaffs(storeId: locker.StoreId)
            .ToListAsync();
        
        foreach (var staff in staffs)
        {
            var notification = new Notification(
                account: staff,
                type: NotificationType.SystemLockerConnected,
                entityType: EntityType.Locker,
                data: locker, 
                saved: true
            );

            await _notifier.NotifyAsync(notification);
        }

        var admins = await _unitOfWork.AccountRepository
            .GetStaffs(roles: new List<Role>() { Role.Admin })
            .ToListAsync();
        
        foreach (var admin in admins)
        {
            var notification = new Notification(
                account: admin,
                type: NotificationType.SystemLockerConnected,
                entityType: EntityType.Locker,
                data: locker, 
                saved: true
            );

            await _notifier.NotifyAsync(notification);
        }
    }
}