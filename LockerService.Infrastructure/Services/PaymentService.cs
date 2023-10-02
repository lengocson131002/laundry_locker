using LockerService.Application.Common.Services;
using LockerService.Application.EventBus.RabbitMq;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Scheduler;
using Quartz;

namespace LockerService.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ILogger<PaymentService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRabbitMqBus _rabbitMqBus;
    private readonly ICurrentAccountService _currentAccountService;
    
    public PaymentService(
        ISchedulerFactory schedulerFactory, 
        ILogger<PaymentService> logger, 
        IUnitOfWork unitOfWork, 
        IRabbitMqBus rabbitMqBus, ICurrentAccountService currentAccountService)
    {
        _schedulerFactory = schedulerFactory;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
        _currentAccountService = currentAccountService;
    }

    public async Task Pay(Order order, PaymentMethod method)
    {
        try
        {
            if (Equals(method, PaymentMethod.Cash))
            {
                var currentAccount = await _currentAccountService.GetCurrentAccount();
                
                var prevStatus = order.Status;
                
                order.Status = OrderStatus.Completed;
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();
                
                await _rabbitMqBus.PublishAsync(new OrderUpdatedStatusEvent()
                {
                    OrderId = order.Id,
                    Status = order.Status,
                    PreviousStatus = prevStatus,
                    StaffId = currentAccount != null && currentAccount.IsStoreStaff 
                        ? currentAccount.Id
                        : null,
                    Time = DateTimeOffset.UtcNow
                });
                
                return;
            }
            
                
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<CheckoutOrderJob>()
                .UsingJobData(CheckoutOrderJob.OrderIdKey, order.Id)
                .UsingJobData(CheckoutOrderJob.MethodKey, method.ToString())
                .Build();
            
            var trigger = TriggerBuilder.Create()
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
        catch (Exception ex)
        {
            _logger.LogError("Schedule to to pay order error {error}", ex.Message);
        }
    }
}