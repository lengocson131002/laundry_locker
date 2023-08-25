using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Common.Utils;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderConfirmedConsumer : IConsumer<OrderConfirmedEvent>
{
    private readonly ILogger<OrderConfirmedConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotifier _notifier;

    public OrderConfirmedConsumer(
        ILogger<OrderConfirmedConsumer> logger, 
        IUnitOfWork unitOfWork, INotifier notifier)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<OrderConfirmedEvent> context)
    {
        var eventMessage = context.Message;
        _logger.LogInformation("Received order confirmed message: {0}", JsonSerializer.Serialize(eventMessage));
        
        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == eventMessage.Id)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Staff)
            .Include(order => order.Bill)
            .Include(order => order.Details)
            .ThenInclude(detail => detail.Service)
            .Include(order => order.Timelines)
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
        
        // Push notification
        var notiData = JsonSerializer.Serialize(order, JsonSerializerUtils.GetGlobalJsonSerializerOptions());
        
        await _notifier.NotifyAsync(
            new Notification()
            {
                Account = order.Sender,
                Type = NotificationType.OrderCreated,
                EntityType = EntityType.Order,
                Content = string.Empty,
                Data = notiData,
                ReferenceId = order.Id.ToString(),
            });
        
        if (order.ReceiverId != null && order.Receiver != null)
        {
            await _notifier.NotifyAsync(new Notification()
            {
                Account = order.Receiver,
                Type = NotificationType.OrderCreated,
                EntityType = EntityType.Order,
                Content = NotificationType.OrderCreated.GetDescription(),
                Data = notiData,
                ReferenceId = order.Id.ToString(),
            });
        }

    }
}