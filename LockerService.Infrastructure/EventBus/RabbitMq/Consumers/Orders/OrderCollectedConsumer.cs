namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderCollectedConsumer : IConsumer<OrderCollectedEvent>
{
    private readonly ILogger<OrderUpdatedStatusConsumer> _logger;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMqttBus _mqttBus;
    
    public OrderCollectedConsumer(
        IUnitOfWork unitOfWork,
        ILogger<OrderUpdatedStatusConsumer> logger,
        IMqttBus mqttBus)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mqttBus = mqttBus;
    }

    public async Task Consume(ConsumeContext<OrderCollectedEvent> context)
    {
        
        var eventMessage = context.Message;
        var order = eventMessage.Order;
        
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
            StaffId = eventMessage.Staff.Id,
            PreviousStatus = eventMessage.PreviousStatus,
            Status = order.Status,
        };
        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("[RABBIT MQ] Handle order collected: {0}", order.Id);

    }
}