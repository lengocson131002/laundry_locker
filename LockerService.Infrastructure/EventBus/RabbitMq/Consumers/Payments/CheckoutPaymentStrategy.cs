using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.EventBus.RabbitMq;
using LockerService.Domain;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Payments;

public class CheckoutPaymentStrategy : IPaymentStrategy
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IRabbitMqBus _rabbitMqBus;

    private readonly ILogger<CheckoutPaymentStrategy> _logger;

    public CheckoutPaymentStrategy(IUnitOfWork unitOfWork, IRabbitMqBus rabbitMqBus, ILogger<CheckoutPaymentStrategy> logger)
    {
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
        _logger = logger;
    }

    public PaymentType Type => PaymentType.Checkout;

    public async Task Handle(Payment payment)
    {
        _logger.LogInformation("Handle checkout order when payment completed");
        if (!Equals(payment.Status, PaymentStatus.Completed))
        {
            return;
        }
        
        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == payment.OrderId)
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
            .ThenInclude(detail => detail.Service)
            .FirstOrDefaultAsync();
        
        if (order == null)
        {
            return;
        }

        // Update order status
        var currStatus = order.Status;
        order.Status = OrderStatus.Completed;
        order.ReceiveAt = DateTimeOffset.UtcNow;
        order.TotalPrice = order.CalculateTotalPrice();

        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        // Push event
        await _rabbitMqBus.PublishAsync(new OrderCompletedEvent()
        {
            Order = order,
            PreviousStatus = currStatus,
            Time = DateTimeOffset.UtcNow
        });
        
        _logger.LogInformation($"Checkout payment {payment.Id} completed. Order {order.Id} completed successfully");
        
    }
}