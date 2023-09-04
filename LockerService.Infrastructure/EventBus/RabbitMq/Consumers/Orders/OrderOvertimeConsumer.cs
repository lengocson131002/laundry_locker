using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Common.Utils;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderOvertimeConsumer : IConsumer<OrderOvertimeEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderOvertimeConsumer> _logger;
    private readonly INotifier _notifier;

    public OrderOvertimeConsumer(IUnitOfWork unitOfWork, ILogger<OrderOvertimeConsumer> logger, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<OrderOvertimeEvent> context)
    {
        var eventMessage = context.Message;
        _logger.LogInformation("Received order overtime message: {0}", JsonSerializer.Serialize(eventMessage));
        
        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == eventMessage.Id)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Locker)
            .Include(order => order.Locker.Store)
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
        
        // Notify customer
        var orderInfoData = JsonSerializer.Serialize(order, JsonSerializerUtils.GetGlobalJsonSerializerOptions());
        
        await _notifier.NotifyAsync(
            new Notification()
            {
                Account = order.Sender,
                Type = NotificationType.OrderOverTime,
                EntityType = EntityType.Order,
                Content = NotificationType.OrderOverTime.GetDescription(),
                Data = orderInfoData,
                ReferenceId = order.Id.ToString(),
            });
        
        if (order.ReceiverId != null && order.Receiver != null)
        {
            await _notifier.NotifyAsync(new Notification()
            {
                Account = order.Receiver,
                Type = NotificationType.OrderOverTime,
                EntityType = EntityType.Order,
                Content = NotificationType.OrderOverTime.GetDescription(),
                Data = orderInfoData,
                ReferenceId = order.Id.ToString(),
            });
        }
        
        // Notify staff managing that locker
        var staffs = await _unitOfWork.StaffLockerRepository.GetStaffs(order.LockerId);
        foreach (var staff in staffs)
        {
            var notification = new Notification()
            {
                Account = staff,
                Type = NotificationType.OrderOverTime,
                Content = NotificationType.OrderOverTime.GetDescription(),
                EntityType = EntityType.Locker,
                ReferenceId = order.Id.ToString(),
                Data = orderInfoData,
            };
            
            await _notifier.NotifyAsync(notification);
        }

    }
}