namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderProcessedConsumer : IConsumer<OrderProcessedEvent>
{
    private readonly ILogger<OrderUpdatedStatusConsumer> _logger;

    private readonly IUnitOfWork _unitOfWork;

    public OrderProcessedConsumer(
        IUnitOfWork unitOfWork,
        ILogger<OrderUpdatedStatusConsumer> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderProcessedEvent> context)
    {
        var eventMessage = context.Message;
        var order = eventMessage.Order;

        // Save timeline
        var timeline = new OrderTimeline
        {
            OrderId = order.Id,
            StaffId = eventMessage.Staff.Id,
            PreviousStatus = eventMessage.PreviousStatus,
            Status = order.Status
        };
        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("[RABBIT MQ] Handle order processed: {0}", order.Id);
    }
}