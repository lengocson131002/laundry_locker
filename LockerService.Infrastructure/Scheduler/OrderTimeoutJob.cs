using LockerService.Application.Common.Persistence.Repositories;
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
        
        var order = await _unitOfWork.OrderRepository
            .Get(order => order.Id == orderId && (order.IsInitialized || order.IsReserved))
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
        
        var currentStatus = order.Status;
        if (order.IsReserved)
        {
            order.TotalPrice = order.ReservationFee;
        }
        
        order.Status = OrderStatus.Canceled;
        order.CompletedAt = DateTimeOffset.UtcNow;
        order.CancelReason = OrderCancelReason.Timeout;
        
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        await _rabbitMqBus.Publish(new OrderCanceledEvent()
        {
            Order = order,
            PreviousStatus = currentStatus,
            Time = DateTimeOffset.Now
        });
        
        _logger.LogInformation("Clear expired order {orderId} at {time}", orderId, DateTimeOffset.UtcNow);
    }
}