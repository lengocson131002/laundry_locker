using LockerService.Application.Common.Persistence;
using LockerService.Domain.Entities;
using LockerService.Domain.Enums;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LockerService.Infrastructure.Scheduler;

public class OrderTimeoutJob : IJob
{
    public const string OrderIdKey = "orderId";
    
    private readonly ILogger<OrderTimeoutJob> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public OrderTimeoutJob(
        ILogger<OrderTimeoutJob> logger, 
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;

        var orderId = dataMap.GetIntValue(OrderIdKey);

        if (orderId == null)
        {
            return;
        }
        
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

        if (order != null && order.IsInitialized)
        {
            
            // Save timeline
            var timeline = new OrderTimeline()
            {
                Order = order,
                PreviousStatus = order.Status,
                Status = OrderStatus.Canceled,
                Time = DateTimeOffset.UtcNow
            };
            await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
            
            order.Status = OrderStatus.Canceled;
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Clear expired order {orderId} at {time}", orderId, DateTimeOffset.UtcNow);
        }

    }
}