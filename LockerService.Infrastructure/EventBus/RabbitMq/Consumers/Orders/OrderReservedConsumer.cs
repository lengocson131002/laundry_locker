using LockerService.Application.Common.Services;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderReservedConsumer : IConsumer<OrderReservedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly ISettingService _settingService;
    private readonly ILogger<OrderReservedConsumer> _logger;
    private readonly IOrderService _orderService;

    public OrderReservedConsumer(IUnitOfWork unitOfWork, 
        IMqttBus mqttBus, 
        ISettingService settingService, 
        ILogger<OrderReservedConsumer> logger, IOrderService orderService)
    {
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _settingService = settingService;
        _logger = logger;
        _orderService = orderService;
    }

    public async Task Consume(ConsumeContext<OrderReservedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received order reserve message: {0}", JsonSerializer.Serialize(message));

        var order = await _unitOfWork.OrderRepository.GetByIdAsync(message.OrderId);
        if (order == null)
        {
            return;
        }
        
        // Create order timeline
        var timeline = new OrderTimeline()
        {
            Order = order,
            Status = message.Status
        };
        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();

        // Set timeout for created order
        var orderSettings = await _settingService.GetSettings<OrderSettings>();
        var cancelTime = order.CreatedAt.AddMinutes(orderSettings.ReservationInitTimeoutInMinutes);

        await _orderService.CancelExpiredOrder(message.OrderId, cancelTime);
    }
}