using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Services;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Common.Utils;
using LockerService.Domain.Entities.Settings;
using Microsoft.Extensions.Configuration;
namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderInitializedConsumer : IConsumer<OrderInitializedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly IOrderService _orderService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderInitializedConsumer> _logger;
    private readonly ISettingService _settingService;
    private readonly INotifier _notifier;

    public OrderInitializedConsumer(IUnitOfWork unitOfWork, 
        IMqttBus mqttBus, 
        IOrderService orderService, 
        IConfiguration configuration, 
        ILogger<OrderInitializedConsumer> logger, 
        ISettingService settingService, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _orderService = orderService;
        _configuration = configuration;
        _logger = logger;
        _settingService = settingService;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<OrderInitializedEvent> context)
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