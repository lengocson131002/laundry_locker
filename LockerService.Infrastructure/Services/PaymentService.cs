using LockerService.Application.Common.Services;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Scheduler;
using Quartz;

namespace LockerService.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(ISchedulerFactory schedulerFactory, ILogger<PaymentService> logger)
    {
        _schedulerFactory = schedulerFactory;
        _logger = logger;
    }

    public async Task Pay(Order order, PaymentMethod method)
    {
        try
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<CheckoutOrderJob>()
                .UsingJobData(CheckoutOrderJob.OrderIdKey, order.Id)
                .UsingJobData(CheckoutOrderJob.MethodKey, method.ToString())
                .Build();
            
            var trigger = TriggerBuilder.Create()
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(30))
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
        catch (Exception ex)
        {
            _logger.LogError("Schedule to to pay order error {error}", ex.Message);
        }
    }
}