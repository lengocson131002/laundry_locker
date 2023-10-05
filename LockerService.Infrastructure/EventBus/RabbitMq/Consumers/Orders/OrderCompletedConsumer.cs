using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Utils;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderCompletedConsumer : IConsumer<OrderCompletedEvent>
{
    private readonly ILogger<OrderCompletedConsumer> _logger;

    private readonly IMqttBus _mqttBus;

    private readonly IUnitOfWork _unitOfWork;

    private readonly INotifier _notifier;


    public OrderCompletedConsumer(ILogger<OrderCompletedConsumer> logger, IMqttBus mqttBus, IUnitOfWork unitOfWork, INotifier notifier)
    {
        _logger = logger;
        _mqttBus = mqttBus;
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var message = context.Message;
        var order = message.Order;
        
        _logger.LogInformation("Handle order completed event");
        
        // Push MQTT to open box
        if (order.ReceiveBox != null && order.ReceiveBoxId != null)
        {
            await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
            {
                LockerCode = order.Locker.Code,
                BoxNumber = order.ReceiveBox.Number
            });
        }
        
        // Push notification
        var notiData = JsonSerializerUtils.Serialize(order);
        await _notifier.NotifyAsync(
            new Notification()
            {
                Account = order.Sender,
                Type = NotificationType.CustomerOrderCompleted,
                EntityType = EntityType.Order,
                Content = NotificationType.CustomerOrderCompleted.GetDescription(),
                Data = notiData,
                ReferenceId = order.Id.ToString(),
            });
        
        if (order.Receiver != null)
        {
            await _notifier.NotifyAsync(new Notification()
            {
                Account = order.Receiver,
                Type = NotificationType.CustomerOrderCompleted,
                EntityType = EntityType.Order,
                Content = NotificationType.CustomerOrderCompleted.GetDescription(),
                Data = notiData,
                ReferenceId = order.Id.ToString(),
            });
        }
        
        // Save timeline
        var timeline = new OrderTimeline()
        {
            OrderId = order.Id,
            PreviousStatus = message.PreviousStatus,
            Status = order.Status
        };
        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();
    }
}