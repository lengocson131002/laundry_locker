using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Utils;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderCanceledConsumer : IConsumer<OrderCanceledEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly INotifier _notifier;

    private readonly ILogger<OrderCanceledConsumer> _logger;

    public OrderCanceledConsumer(IUnitOfWork unitOfWork, INotifier notifier, ILogger<OrderCanceledConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCanceledEvent> context)
    {
        var message = context.Message;
        var order = message.Order;
        
        var notificationData = JsonSerializerUtils.Serialize(order);

        await _notifier.NotifyAsync(new Notification()
        {
            Account = order.Sender,
            Type = NotificationType.CustomerOrderCanceled,
            Content = NotificationType.CustomerOrderCanceled.GetDescription(),
            EntityType = EntityType.Order,
            Data = notificationData,
            ReferenceId = order.Id.ToString()
        });
        
        // Save timeline
        var timeline = new OrderTimeline()
        {
            OrderId = order.Id,
            PreviousStatus = message.PreviousStatus,
            Status = order.Status,
        };

        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("[RABBIT MQ] Handle order canceled: {0}", order.Id);
    }
}