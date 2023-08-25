using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Common.Utils;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderCanceledConsumer : IConsumer<OrderCanceledEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly ILogger<OrderCanceledConsumer> _logger;
    private readonly INotifier _notifier;

    public OrderCanceledConsumer(IUnitOfWork unitOfWork, IMqttBus mqttBus, ILogger<OrderCanceledConsumer> logger, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _logger = logger;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<OrderCanceledEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received order canceled message: {0}", JsonSerializer.Serialize(message));

        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == message.Id)
            .Include(order => order.Locker)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Locker.Location)
            .Include(order => order.Locker.Location.Ward)
            .Include(order => order.Locker.Location.District)
            .Include(order => order.Locker.Location.Province)
            .FirstOrDefaultAsync();
        
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
        
        // Push notification
        var notificationData = JsonSerializer.Serialize(order, JsonSerializerUtils.GetGlobalJsonSerializerOptions());

        await _notifier.NotifyAsync(new Notification()
        {
            Account = order.Sender,
            Type = NotificationType.OrderCanceled,
            Content = NotificationType.OrderCanceled.GetDescription(),
            EntityType = EntityType.Order,
            Data = notificationData,
            ReferenceId = order.Id.ToString()
        });

        if (order.ReceiverId != null && order.Receiver != null)
        {
            await _notifier.NotifyAsync(new Notification()
            {
                Account = order.Receiver,
                Type = NotificationType.OrderCanceled,
                Content = NotificationType.OrderCanceled.GetDescription(),
                EntityType = EntityType.Order,
                Data = notificationData,
                ReferenceId = order.Id.ToString()
            });
        }
    }
}