using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderReservedConsumer : IConsumer<OrderReservedEvent>
{
    private readonly ILockersService _lockersService;

    private readonly ILogger<OrderInitializedConsumer> _logger;

    private readonly IOrderService _orderService;

    private readonly ISettingService _settingService;

    private readonly IUnitOfWork _unitOfWork;

    public OrderReservedConsumer(ILogger<OrderInitializedConsumer> logger,
        IOrderService orderService,
        ISettingService settingService,
        IUnitOfWork unitOfWork,
        ILockersService lockersService)
    {
        _logger = logger;
        _orderService = orderService;
        _settingService = settingService;
        _unitOfWork = unitOfWork;
        _lockersService = lockersService;
    }

    public async Task Consume(ConsumeContext<OrderReservedEvent> context)
    {
        var message = context.Message;
        var order = message.Order;
        var orderTime = message.Time;

        var orderSettings = await _settingService.GetSettings<OrderSettings>();
        var cancelTime = orderTime.AddMinutes(orderSettings.ReservationInitTimeoutInMinutes);
        await _orderService.CancelExpiredOrder(order.Id, cancelTime);

        // Save timeline
        var timeline = new OrderTimeline
        {
            OrderId = order.Id,
            Status = order.Status
        };

        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();

        await _lockersService.CheckLockerBoxAvailability(order.Locker);
        
        _logger.LogInformation("[RABBIT MQ] Handle order reserved: {0}", order.Id);
    }
}