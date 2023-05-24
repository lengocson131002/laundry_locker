using LockerService.Application.Common.Services;
using LockerService.Infrastructure.Scheduler;
using Microsoft.Extensions.Logging;
using Quartz;

namespace LockerService.Infrastructure.Services;

public class OrderTimeoutService : IOrderTimeoutService
{
    private readonly ILogger<OrderTimeoutService> _logger;
    private readonly ISchedulerFactory _schedulerFactory; 
    public OrderTimeoutService(ILogger<OrderTimeoutService> logger, 
        ISchedulerFactory schedulerFactory)
    {
        _logger = logger;
        _schedulerFactory = schedulerFactory;
    }

    public async Task CancelExpiredOrder(int orderId, DateTimeOffset time)
    {
        try
        {
            _logger.LogInformation("Order {orderId} will be expired at {time}", orderId, time);
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<OrderTimeoutJob>()
                .UsingJobData(OrderTimeoutJob.OrderIdKey, orderId)
                .Build();
            
            var trigger = TriggerBuilder.Create()
                .StartAt(time)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
        catch (Exception ex)
        {
            _logger.LogError("Schedule to clear expired order error {error}", ex.Message);
        }
    }
}