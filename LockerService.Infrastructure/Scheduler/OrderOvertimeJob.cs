using LockerService.Application.EventBus.RabbitMq;
using LockerService.Domain.Entities.Settings;
using LockerService.Domain.Enums;
using Quartz;

namespace LockerService.Infrastructure.Scheduler;

public class OrderOvertimeJob : IJob
{
    
    public const string OrderOvertimeJobKey = "OrderOvertimeJobKey";
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderTimeoutJob> _logger;
    private readonly IRabbitMqBus _rabbitMqBus;
    private readonly int _numberOfOrder = 100;
    public OrderOvertimeJob(IUnitOfWork unitOfWork, ILogger<OrderTimeoutJob> logger, IRabbitMqBus rabbitMqBus)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Scan overtime orders");
        
        // Get all overtime orders
        var overtimeOrders = await _unitOfWork
            .OrderRepository
            .GetOvertimeOrders()
            .Take(_numberOfOrder)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Locker)
            .Include(order => order.Locker.Location)
            .Include(order => order.Locker.Location.Ward)
            .Include(order => order.Locker.Location.District)
            .Include(order => order.Locker.Location.Province)
            .ToListAsync();

        foreach (var order in overtimeOrders)
        {
            _logger.LogInformation("Order {0} is overtime", order.Id);
            
            var currentStatus = order.Status;
            
            order.Status = OrderStatus.Overtime;
            await _unitOfWork.OrderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            await _rabbitMqBus.PublishAsync(new OrderOvertimeEvent()
            {
                Order = order,
                PreviousStatus = currentStatus,
                Time = DateTimeOffset.Now
            });
        }

    }
}