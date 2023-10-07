using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using Quartz;

namespace LockerService.Infrastructure.Scheduler;

public class CheckoutOrderJob : IJob
{
    public static string OrderIdKey = "orderId";
    public static string PaymentIdKey = "paymentId";
    
    private readonly ILogger<OrderTimeoutJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _rabbitMqBus;

    public CheckoutOrderJob(ILogger<OrderTimeoutJob> logger, IUnitOfWork unitOfWork, IPublishEndpoint rabbitMqBus)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;
        var orderId = dataMap.GetLongValue(OrderIdKey);
        var paymentId = dataMap.GetLongValue(PaymentIdKey);
        
        if (orderId == 0 || paymentId == 0)
        {
            return;
        }

        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == orderId)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Locker.Location)
            .Include(order => order.Locker.Location.Ward)
            .Include(order => order.Locker.Location.District)
            .Include(order => order.Locker.Location.Province)
            .Include(order => order.Details)
            .FirstOrDefaultAsync();
        
        if (order == null)
        {
            return;
        }

        var payment = await _unitOfWork.PaymentRepository
            .Get(pay => pay.Id == paymentId)
            .FirstOrDefaultAsync();

        if (payment == null)
        {
            return;
        }

        // Update payment status
        payment.Status = PaymentStatus.Completed;
        
        // Update order status
        var currStatus = order.Status;
        order.Status = OrderStatus.Completed;
        order.CompletedAt = DateTimeOffset.UtcNow;
        order.TotalPrice = order.CalculateTotalPrice();

        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        // Push event
        await _rabbitMqBus.Publish(new OrderCompletedEvent()
        {
            Order = order,
            PreviousStatus = currStatus,
            Time = DateTimeOffset.UtcNow
        });
        
        _logger.LogInformation("Completed order {0}.", order.Id);

    }
}