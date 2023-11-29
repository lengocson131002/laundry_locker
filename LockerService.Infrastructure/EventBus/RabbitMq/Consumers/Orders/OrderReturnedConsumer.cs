using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Shared.Extensions;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderReturnedConsumer : IConsumer<OrderReturnedEvent>
{
    private readonly ILogger<OrderInitializedConsumer> _logger;

    private readonly IMqttBus _mqttBus;

    private readonly IOrderService _orderService;

    private readonly ISettingService _settingService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILockersService _lockersService;

    private readonly INotifier _notifier;
    
    public OrderReturnedConsumer(ILogger<OrderInitializedConsumer> logger,
        IOrderService orderService,
        IMqttBus mqttBus,
        ISettingService settingService,
        IUnitOfWork unitOfWork, ILockersService lockersService, INotifier notifier)
    {
        _logger = logger;
        _orderService = orderService;
        _mqttBus = mqttBus;
        _settingService = settingService;
        _unitOfWork = unitOfWork;
        _lockersService = lockersService;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<OrderReturnedEvent> context)
    {
        var message = context.Message;
        var order = message.Order;
        var staff = message.Staff;
        
        // Mqtt Open Box
        if (order.ReceiveBox != null && order.ReceiveBoxId != null)
        {
            await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
            {
                LockerCode = order.Locker.Code,
                BoxNumber = order.ReceiveBox.Number
            });
        }
        
        // Push notification for receiver
        if (!order.DeliverySupported)
        {
            var notiAccount = order.ReceiverId != null && order.Receiver != null 
                ? order.Receiver 
                : order.Sender;
            
            await _notifier.NotifyAsync(
                new Notification(
                    account: notiAccount,
                    type: NotificationType.CustomerOrderReturned,
                    entityType: EntityType.Order,
                    data: order
                ));
        }
        
        // Check locker box availability
        await _lockersService.CheckLockerBoxAvailability(order.Locker);
        
        // Save timeline
        var timeline = new OrderTimeline()
        {
            OrderId = order.Id,
            StaffId = staff.Id,
            PreviousStatus = message.PreviousStatus,
            Status = order.Status,
        };

        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("[RABBIT MQ] Handle order returned: {0}", order.Id);
    }
}