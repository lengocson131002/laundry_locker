using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Shared.Extensions;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderConfirmedConsumer : IConsumer<OrderConfirmedEvent>
{
    private readonly ILockersService _lockersService;

    private readonly ILogger<OrderUpdatedStatusConsumer> _logger;

    private readonly INotifier _notifier;
    
    private readonly IUnitOfWork _unitOfWork;

    public OrderConfirmedConsumer(
        IUnitOfWork unitOfWork,
        ILogger<OrderUpdatedStatusConsumer> logger,
        INotifier notifier, 
        ILockersService lockersService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _notifier = notifier;
        _lockersService = lockersService;
    }

    public async Task Consume(ConsumeContext<OrderConfirmedEvent> context)
    {
        var eventMessage = context.Message;
        
        var order = eventMessage.Order;

        // Push notification whenever order initialized
        if (Equals(eventMessage.PreviousStatus, OrderStatus.Initialized))
        {
            await _notifier.NotifyAsync(
                new Notification(
                    account: order.Sender,
                    type: NotificationType.CustomerOrderCreated,
                    entityType: EntityType.Order,
                    data: order
                ));

            if (order.ReceiverId != null && order.Receiver != null)
            {
                await _notifier.NotifyAsync(
                    new Notification(
                        account: order.Receiver,
                        type: NotificationType.CustomerOrderCreated,
                        entityType: EntityType.Order,
                        data: order
                    ));
            }


            // Notify for staffs to collect when order type is laundry
            if (order.IsLaundry)
            {
                var staffs = await _unitOfWork.AccountRepository
                    .GetStaffs(storeId: order.Locker.StoreId)
                    .ToListAsync();

                foreach (var staff in staffs)
                {
                    var notification = new Notification(
                        account: staff,
                        type: NotificationType.SystemOrderCreated,
                        entityType: EntityType.Order,
                        data: order
                    );

                    await _notifier.NotifyAsync(notification);
                }
            }
        }

        // Save timeline
        var timeline = new OrderTimeline()
        {
            OrderId = order.Id,
            PreviousStatus = eventMessage.PreviousStatus,
            Status = order.Status,
        };

        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();
        
        // Check locker box availability
        await _lockersService.CheckLockerBoxAvailability(order.Locker);
        
        _logger.LogInformation("[RABBIT MQ] Handle order confirmed: {0}", order.Id);

    }
}