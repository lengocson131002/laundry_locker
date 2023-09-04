using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Services;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Common.Utils;
using LockerService.Domain.Entities.Settings;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderReturnedConsumer : IConsumer<OrderReturnedEvent>
{
    private readonly ILogger<OrderConfirmedConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotifier _notifier;
    private readonly IMqttBus _mqttBus;
    private readonly ISettingService _settingService;
    
    public OrderReturnedConsumer(
        ILogger<OrderConfirmedConsumer> logger, 
        IUnitOfWork unitOfWork, 
        IMqttBus mqttBus, 
        INotifier notifier, 
        ISettingService settingService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _notifier = notifier;
        _settingService = settingService;
    }
    
    public async Task Consume(ConsumeContext<OrderReturnedEvent> context)
    {
        var eventMessage = context.Message;
        _logger.LogInformation("Received order returned message: {0}", JsonSerializer.Serialize(eventMessage));

        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == eventMessage.Id)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Details)
            .ThenInclude(detail => detail.Service)
            .Include(order => order.Locker.Location)
            .Include(order => order.Locker.Location.Ward)
            .Include(order => order.Locker.Location.District)
            .Include(order => order.Locker.Location.Province)
            .FirstOrDefaultAsync();
        
        if (order == null)
        {
            return;
        }
        
        // Save timeline
        var timeline = new OrderTimeline()
        {
            Order = order,
            PreviousStatus = eventMessage.PreviousStatus,
            Status = eventMessage.Status
        };
        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();

        // Mqtt Open Box
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
        {
            LockerCode = order.Locker.Code,
            BoxNumber = order.ReceiveBox.Number
        });
        
        // Push notification
        var orderInfoData = JsonSerializer.Serialize(order, JsonSerializerUtils.GetGlobalJsonSerializerOptions());
        await _notifier.NotifyAsync(
            new Notification()
            {
                Account = order.Sender,
                Type = NotificationType.OrderReturned,
                EntityType = EntityType.Order,
                Content = NotificationType.OrderReturned.GetDescription(),
                Data = orderInfoData,
                ReferenceId = order.Id.ToString(),
            });
        
        if (order.Receiver != null)
        {
            await _notifier.NotifyAsync(new Notification()
            {
                Account = order.Receiver,
                Type = NotificationType.OrderReturned,
                EntityType = EntityType.Order,
                Content = NotificationType.OrderReturned.GetDescription(),
                Data = orderInfoData,
                ReferenceId = order.Id.ToString(),
            });
        }
        
        // Check locker box availability
        var lockerSettings = await _settingService.GetSettings<LockerSettings>();
        var availableBoxes = await _unitOfWork.BoxRepository.FindAvailableBoxes(order.LockerId);
        if (availableBoxes.Count <= lockerSettings.AvailableBoxCountWarning)
        {
            var staffs = await _unitOfWork.StaffLockerRepository.GetStaffs(order.LockerId);
            var lockerInfoData = JsonSerializer.Serialize(order.Locker, JsonSerializerUtils.GetGlobalJsonSerializerOptions());
            foreach (var staff in staffs)
            {
                var notification = new Notification()
                {
                    Account = staff,
                    Type = NotificationType.LockerBoxWarning,
                    Content = NotificationType.LockerBoxWarning.GetDescription(),
                    EntityType = EntityType.Locker,
                    ReferenceId = order.LockerId.ToString(),
                    Data = lockerInfoData,
                };
            
                await _notifier.NotifyAsync(notification);
            }
        }
    }
}