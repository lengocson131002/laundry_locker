using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderOvertimeProcessingConsumer : IConsumer<OrderOvertimeProcessingEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<OrderOvertimeConsumer> _logger;
    
    private readonly IMqttBus _mqttBus;

    private readonly INotifier _notifier;

    public OrderOvertimeProcessingConsumer(IUnitOfWork unitOfWork, ILogger<OrderOvertimeConsumer> logger, IMqttBus mqttBus, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mqttBus = mqttBus;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<OrderOvertimeProcessingEvent> context)
    {
        var message = context.Message;

        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == message.OrderId)
            .Include(order => order.Locker)
            .Include(order => order.Receiver)
            .Include(order => order.Sender)
            .Include(order => order.ReceiveBox)
            .Include(order => order.SendBox)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            return;
        }

        // Push notification to customer
        await _notifier.NotifyAsync(
            new Notification(
                account: order.Sender,
                type: NotificationType.CustomerOrderOverTimeProcessing,
                entityType: EntityType.Order,
                data: order
            ));
        
        if (order.ReceiverId != null && order.Receiver != null)
        {
            await _notifier.NotifyAsync(new Notification(
                account: order.Receiver,
                type: NotificationType.CustomerOrderOverTimeProcessing,
                entityType: EntityType.Order,
                data: order
            ));
        }
        
        // Push MQTT to open box
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
        {
            LockerCode = order.Locker.Code,
            BoxNumber = message.ReceiveBoxNumber
        });
        
        // Save timeline
        var timeline = new OrderTimeline()
        {
            OrderId = order.Id,
            StaffId = message.Staff.Id,
            PreviousStatus = message.PreviousStatus,
            Status = order.Status,
        };

        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("[RABBIT MQ] Handle order overtime processed: {0}", order.Id);
    }
}