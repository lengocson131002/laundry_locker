using LockerService.Application.Common.Services;
using LockerService.Domain.Enums;
using Quartz;

namespace LockerService.Infrastructure.Scheduler;

public class CheckoutOrderJob : IJob
{
    public static string OrderIdKey = "orderId";
    public static string MethodKey = "method";
    
    private readonly ILogger<OrderTimeoutJob> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _rabbitMqBus;
    private readonly IOrderService _orderService;

    public CheckoutOrderJob(ILogger<OrderTimeoutJob> logger, IUnitOfWork unitOfWork, IPublishEndpoint rabbitMqBus, IOrderService orderService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
        _orderService = orderService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;
        var orderId = dataMap.GetLongValue(OrderIdKey);
        
        if (orderId == 0 || !Enum.TryParse(dataMap.GetString(MethodKey), out PaymentMethod method))
        {
            return;
        }

        var order = await _unitOfWork.OrderRepository.Get(
            predicate: order => order.Id == orderId,
            includes: new List<Expression<Func<Order, object>>>()
            {
                order => order.Details
            }).FirstOrDefaultAsync();
        
        if (order == null || !order.CanCheckout)
        {
            return;
        }

        var currStatus = order.Status;
        
        await _orderService.CalculateFree(order);
        
        var bill = Bill.CreateBill(order, method);
        order.Bill = bill;
        order.Status = OrderStatus.Completed;
        
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        // Push event
        await _rabbitMqBus.Publish(new OrderCompletedEvent()
        {
            Id = order.Id,
            PreviousStatus = currStatus,
            Status = order.Status
        });
        
        _logger.LogInformation("Completed order {0}.", order.Id);

    }
}