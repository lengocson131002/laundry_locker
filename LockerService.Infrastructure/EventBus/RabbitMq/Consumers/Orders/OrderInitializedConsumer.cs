using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderInitializedConsumer : IConsumer<OrderInitializedEvent>
{
    private readonly ILogger<OrderInitializedConsumer> _logger;

    private readonly IOrderService _orderService;

    private readonly IMqttBus _mqttBus;

    private readonly ISettingService _settingService;

    private readonly IUnitOfWork _unitOfWork;

    public OrderInitializedConsumer(ILogger<OrderInitializedConsumer> logger, IOrderService orderService, IMqttBus mqttBus, ISettingService settingService, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _orderService = orderService;
        _mqttBus = mqttBus;
        _settingService = settingService;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<OrderInitializedEvent> context)
    {
        var message = context.Message;
        var order = message.Order;
        
        var orderSettings = await _settingService.GetSettings<OrderSettings>();
        var cancelTime = message.Time.AddMinutes(orderSettings.InitTimeoutInMinutes);

        await _orderService.CancelExpiredOrder(order.Id, cancelTime);
        
        // Push MQTT to open box
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
        {
            LockerCode = order.Locker.Code,
            BoxNumber = order.SendBox.Number
        });

        // Save timeline
        var timeline = new OrderTimeline()
        {
            OrderId = order.Id,
            Status = order.Status,
        };

        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("[RABBIT MQ] Handle order initialized: {0}", order.Id);

    }
}