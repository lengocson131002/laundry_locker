using System.Text.Json;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using MassTransit;

namespace LockerService.Application.EventBus.RabbitMq.Consumers.Orders;

public class OrderCompletedConsumer : IConsumer<OrderCompletedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;
    private readonly ILogger<OrderCompletedConsumer> _logger;

    public OrderCompletedConsumer(IUnitOfWork unitOfWork, IMqttBus mqttBus, ILogger<OrderCompletedConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received order completed message: {0}", JsonSerializer.Serialize(message));

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
        
        // Push MQTT to open box
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent(order.LockerId, order.ReceiveBox.Number));
    }
}