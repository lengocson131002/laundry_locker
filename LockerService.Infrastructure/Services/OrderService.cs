using LockerService.Application.Common.Enums;
using LockerService.Application.Common.Exceptions;
using LockerService.Application.Common.Services;
using LockerService.Domain.Entities.Settings;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Scheduler;
using Quartz;

namespace LockerService.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ISettingService _settingService;

    private readonly ILogger<OrderService> _logger;
    
    private readonly ISchedulerFactory _schedulerFactory;

    private const int MinStorageDurationInHour = 1; 

    public OrderService(IUnitOfWork unitOfWork, ISettingService settingService, ILogger<OrderService> logger, ISchedulerFactory schedulerFactory)
    {
        _unitOfWork = unitOfWork;
        _settingService = settingService;
        _logger = logger;
        _schedulerFactory = schedulerFactory;
    }

    public async Task CalculateFree(Order order)
    {
        switch (order.Type)
        {
            case OrderType.Storage:
                await CalculateStoreFee(order);
                break;
            case OrderType.Laundry:
                await CalculateLaundryFree(order);
                break;
        }
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

    private async Task CalculateStoreFee(Order order)
    {
        var orderSettings = await _settingService.GetSettings<OrderSettings>();
        var duration = Math.Max(MinStorageDurationInHour, GetOrderDuration(order));
        order.Price = (decimal) duration * orderSettings.StoragePrice;
    }

    private async Task CalculateLaundryFree(Order order)
    {
        if (!order.UpdatedInfo)
        {
            throw new ApiException(ResponseCode.OrderDetailErrorInfoRequired);
        }

        var orderSettings = await _settingService.GetSettings<OrderSettings>();
        var servicePrice = order.Details.Sum(item => item.Price * (decimal) item.Quantity!);
        order.Price = Round(servicePrice);
        
        // Calculate extra fee
        var duration = GetOrderDuration(order);
        var maxTime = orderSettings.MaxTimeInHours;
        if (duration > maxTime)
        {
            order.ExtraCount = duration - maxTime;
            order.ExtraFee = orderSettings.ExtraFee;
        }
    }

    private decimal Round(decimal fee)
    {
        return Math.Round(fee, 2, MidpointRounding.AwayFromZero);
    }

    private float GetOrderDuration(Order order)
    {
        var timespan = DateTimeOffset.UtcNow - order.CreatedAt;
        var totalHours = (float) timespan.TotalMinutes / 60;
        return (float) Math.Round(totalHours, 2, MidpointRounding.AwayFromZero);
    }
    
}