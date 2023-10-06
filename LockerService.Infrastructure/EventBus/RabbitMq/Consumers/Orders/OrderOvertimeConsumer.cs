using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Shared.Extensions;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderOvertimeConsumer : IConsumer<OrderOvertimeEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly INotifier _notifier;

    private readonly ILogger<OrderOvertimeConsumer> _logger;

    public OrderOvertimeConsumer(IUnitOfWork unitOfWork, INotifier notifier, ILogger<OrderOvertimeConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderOvertimeEvent> context)
    {
        var message = context.Message;
        var order = message.Order;
            
        var orderInfoData = JsonSerializerUtils.Serialize(order);
        
        await _notifier.NotifyAsync(
            new Notification()
            {
                Account = order.Sender,
                Type = NotificationType.CustomerOrderOverTime,
                EntityType = EntityType.Order,
                Content = NotificationType.CustomerOrderOverTime.GetDescription(),
                Data = orderInfoData,
                ReferenceId = order.Id.ToString(),
            });
        
        if (order.ReceiverId != null && order.Receiver != null)
        {
            await _notifier.NotifyAsync(new Notification()
            {
                Account = order.Receiver,
                Type = NotificationType.CustomerOrderOverTime,
                EntityType = EntityType.Order,
                Content = NotificationType.CustomerOrderOverTime.GetDescription(),
                Data = orderInfoData,
                ReferenceId = order.Id.ToString(),
            });
        }
        
        // Notify manager to handle this order
        var laundryAttendants = await _unitOfWork.AccountRepository
            .GetStaffs(
                storeId: order.Locker.StoreId, 
                role: Role.LaundryAttendant)
            .ToListAsync();;
        
        foreach (var la in laundryAttendants)
        {
            var notification = new Notification()
            {
                Account = la,
                Type = NotificationType.SystemOrderOverTime,
                Content = NotificationType.SystemOrderOverTime.GetDescription(),
                EntityType = EntityType.Locker,
                ReferenceId = order.Id.ToString(),
                Data = orderInfoData,
            };
            
            await _notifier.NotifyAsync(notification);
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
        
        _logger.LogInformation("[RABBIT MQ] Handle order overtime: {0}", order.Id);
    }
}