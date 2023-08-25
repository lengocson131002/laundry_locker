using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Common.Utils;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderCompletedConsumer : IConsumer<OrderCompletedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly ILogger<OrderCompletedConsumer> _logger;
    private readonly INotifier _notifier;

    public OrderCompletedConsumer(IUnitOfWork unitOfWork, IMqttBus mqttBus, ILogger<OrderCompletedConsumer> logger, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _logger = logger;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received order completed message: {0}", JsonSerializer.Serialize(message));

        var orderQuery = await _unitOfWork.OrderRepository.GetAsync(
            predicate: order => order.Id == message.Id,
            includes: new List<Expression<Func<Order, object>>>()
            {
                order => order.Sender,
                order => order.Receiver,
                order => order.SendBox,
                order => order.ReceiveBox,
                order => order.Locker
            });
        var order = await orderQuery.FirstOrDefaultAsync();
        if (order == null)
        {
            return;
        }
        
        // Create order timeline
        var timeline = new OrderTimeline()
        {
            Order = order,
            Status = message.Status,
            PreviousStatus = message.PreviousStatus
        };
        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();
        
        // Push MQTT to open box
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
        {
            LockerCode = order.Locker.Code,
            BoxNumber = order.ReceiveBox.Number
        });
        
        // Push notification
        var notiData = JsonSerializer.Serialize(order, JsonSerializerUtils.GetGlobalJsonSerializerOptions());
        await _notifier.NotifyAsync(
            new Notification()
            {
                Account = order.Sender,
                Type = NotificationType.OrderCompleted,
                EntityType = EntityType.Order,
                Content = NotificationType.OrderCompleted.GetDescription(),
                Data = notiData,
                ReferenceId = order.Id.ToString(),
            });
        
        if (order.Receiver != null)
        {
            await _notifier.NotifyAsync(new Notification()
            {
                Account = order.Receiver,
                Type = NotificationType.OrderCompleted,
                EntityType = EntityType.Order,
                Content = NotificationType.OrderCompleted.GetDescription(),
                Data = notiData,
                ReferenceId = order.Id.ToString(),
            });
        }
    }
}