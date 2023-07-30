using LockerService.Application.Common.Services;
using LockerService.Domain.Entities.Settings;
using LockerService.Infrastructure.Common;
using Microsoft.Extensions.Configuration;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly IOrderService _orderService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly ISettingService _settingService;

    public OrderCreatedConsumer(IUnitOfWork unitOfWork, 
        IMqttBus mqttBus, 
        IOrderService orderService, 
        IConfiguration configuration, 
        ILogger<OrderCreatedConsumer> logger, 
        ISettingService settingService)
    {
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _orderService = orderService;
        _configuration = configuration;
        _logger = logger;
        _settingService = settingService;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received order created message: {0}", JsonSerializer.Serialize(message));

        var order = await _unitOfWork.OrderRepository
            .Get(predicate: order => order.Id == message.OrderId,
            includes: new List<Expression<Func<Order, object>>>()
            {
                order => order.Locker,
                order => order.SendBox
            })
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

        // Set timeout for created order
        var orderSettings = await _settingService.GetSettings<OrderSettings>();
        var cancelTime = message.Time.AddMinutes(orderSettings.InitTimeoutInMinutes);

        await _orderService.CancelExpiredOrder(message.OrderId, cancelTime);
        
        // Push MQTT to open box
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
        {
            LockerCode = order.Locker.Code,
            BoxNumber = order.SendBox.Number
        });
    }
}