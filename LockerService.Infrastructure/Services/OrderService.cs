using LockerService.Infrastructure.Scheduler;
using Quartz;

namespace LockerService.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ISettingService _settingService;
    
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork, ISettingService settingService, ILogger<OrderService> logger,
        ISchedulerFactory schedulerFactory)
    {
        _unitOfWork = unitOfWork;
        _settingService = settingService;
        _logger = logger;
        _schedulerFactory = schedulerFactory;
    }

    public async Task CancelExpiredOrder(long orderId, DateTimeOffset time)
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