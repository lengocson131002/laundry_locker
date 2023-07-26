using LockerService.Application.Common.Services;
using LockerService.Infrastructure.Common;
using Microsoft.Extensions.Configuration;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly IOrderTimeoutService _orderTimeoutService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(IUnitOfWork unitOfWork, 
        IMqttBus mqttBus, 
        IOrderTimeoutService orderTimeoutService, 
        IConfiguration configuration, 
        ILogger<OrderCreatedConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _orderTimeoutService = orderTimeoutService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received order created message: {0}", JsonSerializer.Serialize(message));

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
        var cancelTime = message.Time
            .AddMinutes(_configuration.GetValueOrDefault("Order:TimeoutInMinutes", BaseConstants.DefaultOrderTimeoutInMinutes));

        await _orderTimeoutService.CancelExpiredOrder(message.OrderId, cancelTime);
    }
}