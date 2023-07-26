using System.Text.Json;
using LockerService.Application.Common.Services.Notification;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using MassTransit;

namespace LockerService.Application.EventBus.RabbitMq.Consumers.Orders;

public class OrderConfirmedConsumer : IConsumer<OrderConfirmedEvent>
{
    private readonly ILogger<OrderConfirmedConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISmsNotificationService _smsNotificationService;

    public OrderConfirmedConsumer(
        ILogger<OrderConfirmedConsumer> logger, 
        IUnitOfWork unitOfWork, 
        ISmsNotificationService smsNotificationService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _smsNotificationService = smsNotificationService;
    }

    public async Task Consume(ConsumeContext<OrderConfirmedEvent> context)
    {
        var eventMessage = context.Message;
        _logger.LogInformation("Received order confirmed message: {0}", JsonSerializer.Serialize(eventMessage));
        
        var orderQuery = await _unitOfWork.OrderRepository.GetAsync(
            predicate: order => order.Id == eventMessage.Id,
            includes: new List<Expression<Func<Order, object>>>()
            {
                order => order.Locker,
                order => order.Locker.Location,
                order => order.Locker.Location.Ward,
                order => order.Locker.Location.District,
                order => order.Locker.Location.Province
            });
        
        var order = await orderQuery.FirstOrDefaultAsync();
        if (order == null)
        {
            return;
        }

        // Save timeline
        var timeline = new OrderTimeline()
        {
            Order = order,
            PreviousStatus = eventMessage.PreviousStatus,
            Status = eventMessage.Status
        };
        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();
    }
}