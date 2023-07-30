namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderProcessingConsumer : IConsumer<OrderProcessingEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly ILogger<OrderProcessingConsumer> _logger;

    public OrderProcessingConsumer(IUnitOfWork unitOfWork, IMqttBus mqttBus, ILogger<OrderProcessingConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderProcessingEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received order processing message: {0}", JsonSerializer.Serialize(message));

        var order = await _unitOfWork.OrderRepository
            .Get(
                predicate: order => order.Id == message.Id,
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
        
        // Save timeline
        var orderTimeline = new OrderTimeline()
        {
            OrderId = message.Id,
            PreviousStatus = message.PreviousStatus,
            Status = message.Status
        };
        await _unitOfWork.OrderTimelineRepository.AddAsync(orderTimeline);
        await _unitOfWork.SaveChangesAsync();
        
        // Push MQTT to open box
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
        {
            LockerCode = order.Locker.Code,
            BoxNumber = order.SendBox.Number
        });
    }
}