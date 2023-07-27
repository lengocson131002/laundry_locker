namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderCanceledConsumer : IConsumer<OrderCanceledEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly ILogger<OrderCanceledConsumer> _logger;

    public OrderCanceledConsumer(IUnitOfWork unitOfWork, IMqttBus mqttBus, ILogger<OrderCanceledConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCanceledEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received order canceled message: {0}", JsonSerializer.Serialize(message));

        var order = await _unitOfWork.OrderRepository.GetByIdAsync(message.Id);
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
    }
}