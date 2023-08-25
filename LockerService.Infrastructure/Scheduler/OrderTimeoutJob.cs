using LockerService.Domain.Enums;
using Quartz;

namespace LockerService.Infrastructure.Scheduler;

public class OrderTimeoutJob : IJob
{
    public const string OrderIdKey = "orderId";
    
    private readonly ILogger<OrderTimeoutJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _rabbitMqBus;

    public OrderTimeoutJob(
        ILogger<OrderTimeoutJob> logger, 
        IUnitOfWork unitOfWork, IPublishEndpoint rabbitMqBus)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;

        var orderId = dataMap.GetLongValue(OrderIdKey);

        if (orderId == 0)
        {
            return;
        }
        
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
        if (order == null || !order.IsInitialized)
        {
            return;
        }
        
        var currentStatus = order.Status;
        
        order.Status = OrderStatus.Canceled;
        order.CancelReason = OrderCancelReason.Timeout;
        
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        await _rabbitMqBus.Publish(new OrderCanceledEvent()
        {
            Id = orderId,
            PreviousStatus = currentStatus,
            Status = order.Status
        });
        
        _logger.LogInformation("Clear expired order {orderId} at {time}", orderId, DateTimeOffset.UtcNow);
    }
}