using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Shared.Extensions;
using LockerService.Shared.Utils;

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

        await _notifier.NotifyAsync(new Notification(
            account: order.Sender,
            type: NotificationType.CustomerOrderCanceled,
            entityType: EntityType.Order,
            data: order
        ));
        
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