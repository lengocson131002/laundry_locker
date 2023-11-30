using LockerService.Application.Common.Services.Payments;
using LockerService.Domain;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Scheduler;
using Quartz;

namespace LockerService.Infrastructure.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IMomoPaymentService _momoPaymentService;
    
    private readonly IVnPayPaymentService _vnPayPaymentService;

    private readonly ILogger<PaymentService> _logger;

    private readonly ISchedulerFactory _schedulerFactory;
    
    public PaymentService(IMomoPaymentService momoPaymentService, IVnPayPaymentService vnPayPaymentService, ILogger<PaymentService> logger, ISchedulerFactory schedulerFactory)
    {
        _momoPaymentService = momoPaymentService;
        _vnPayPaymentService = vnPayPaymentService;
        _logger = logger;
        _schedulerFactory = schedulerFactory;
    }

    public async Task<Payment> Checkout(Order order, PaymentMethod method, CancellationToken cancellationToken)
    {
        return method switch
        {
            PaymentMethod.Cash => await HandleCashCheckout(order, cancellationToken),
            PaymentMethod.Momo => await HandleMomoCheckout(order, cancellationToken),
            PaymentMethod.VnPay => await HandleVnPayCheckout(order, cancellationToken),
            _ => throw new Exception("Invalid payment method")
        };
    }

    private async Task<Payment> HandleVnPayCheckout(Order order, CancellationToken cancellationToken)
    {
        var amount = (long)(order.CalculateTotalPrice() - order.ReservationFee);
        var payment = await CreatePayment(new VnPayPayment()
        {
            Amount = amount,
            PaymentReferenceId = Guid.NewGuid().ToString(),
            Info = Payment.PaymentContent(order.Type),
            Time = DateTimeOffset.UtcNow
        });

        payment.OrderId = order.Id;
        payment.CustomerId = order.ReceiverId ?? order.SenderId;
        payment.StoreId = order.Locker.StoreId;
        
        return payment;
    }

    private async Task<Payment> HandleMomoCheckout(Order order, CancellationToken cancellationToken)
    {
        var amount = (long) (order.CalculateTotalPrice() - order.ReservationFee);
        
        var payment = await CreatePayment(new MomoPayment()
        {
            Amount = amount,
            PaymentReferenceId = Guid.NewGuid().ToString(),
            Info =  Payment.PaymentContent(order.Type)
        });

        payment.OrderId = order.Id;
        payment.CustomerId = order.ReceiverId ?? order.SenderId;
        payment.StoreId = order.Locker.StoreId;
        
        return payment;
    }

    private async Task<Payment> HandleCashCheckout(Order order, CancellationToken cancellationToken)
    {
        var payment = new Payment(order, PaymentMethod.Cash, PaymentStatus.Completed);
        return await Task.FromResult(payment);
    }
    
    public async Task SetPaymentTimeOut(Payment payment, DateTimeOffset time)
    {
        try
        {
            _logger.LogInformation("Payment {paymentId} will be expired at {time}", payment.Id, time);
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<PaymentTimeoutJob>()
                .UsingJobData(PaymentTimeoutJob.PaymentIdKey, payment.Id)
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartAt(time)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
        catch (Exception ex)
        {
            _logger.LogError("Schedule to clear expired payment error {error}", ex.Message);
        }
    }

    public async Task<Payment> CreatePayment(MomoPayment momoPayment)
    {
        return await _momoPaymentService.CreatePayment(momoPayment);
    }

    public async Task<Payment> CreatePayment(VnPayPayment vnPayPayment)
    {
        return await _vnPayPaymentService.CreatePayment(vnPayPayment);
    }
}