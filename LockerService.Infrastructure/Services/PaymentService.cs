using LockerService.Application.EventBus.RabbitMq;
using LockerService.Domain;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Scheduler;
using Quartz;

namespace LockerService.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRabbitMqBus _rabbitMqBus;
    
    public PaymentService(
        ISchedulerFactory schedulerFactory, 
        IUnitOfWork unitOfWork, 
        IRabbitMqBus rabbitMqBus)
    {
        _schedulerFactory = schedulerFactory;
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task<Payment> Pay(Order order, PaymentMethod method)
    {
        var payment = await InitPayment(order, method);

        if (Equals(payment.Method, PaymentMethod.Cash))
        {
            var prevStatus = order.Status;

            payment.Status = PaymentStatus.Completed;
            order.Status = OrderStatus.Completed;
            order.TotalPrice = order.Price
                               + order.TotalExtraFee
                               + order.ShippingFee
                               - order.Discount;
            
            await _unitOfWork.OrderRepository.UpdateAsync(order);
            
            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            
            await _rabbitMqBus.PublishAsync(new OrderCompletedEvent()
            {
                Order = order,
                PreviousStatus = prevStatus,
                Time = DateTimeOffset.UtcNow
            });

            return payment;
        }

        await _unitOfWork.PaymentRepository.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        // Test only
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.Start();

        var job = JobBuilder.Create<CheckoutOrderJob>()
            .UsingJobData(CheckoutOrderJob.OrderIdKey, order.Id)
            .UsingJobData(CheckoutOrderJob.PaymentIdKey, payment.Id)
            .Build();
        
        var trigger = TriggerBuilder.Create()
            .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
            .Build();
        
        await scheduler.ScheduleJob(job, trigger);

        return payment;
    }

    public async Task<Payment> InitPayment(Order order, PaymentMethod method)
    {
        var payment = new Payment(order, method);
        return await Task.FromResult(payment);
    }

}