using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.Features.Payments.Commands;

namespace LockerService.Application.Features.Payments.Handlers;

public class VnPayPaymentCallbackHandler : IRequestHandler<VnPayPaymentCallback>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IRabbitMqBus _rabbitMqBus;

    private readonly ILogger<MomoPaymentCallbackHandler> _logger;
    
    public VnPayPaymentCallbackHandler(IUnitOfWork unitOfWork, IRabbitMqBus rabbitMqBus, ILogger<MomoPaymentCallbackHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
        _logger = logger;
    }

    public async Task Handle(VnPayPaymentCallback request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.PaymentRepository
            .Get(payment => payment.ReferenceId == request.PaymentReferenceId && Equals(payment.Status, PaymentStatus.Created))
            .FirstOrDefaultAsync(cancellationToken);

        if (payment == null)
        {
            throw new ApiException(ResponseCode.PaymentErrorNotFound);
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
            .FirstOrDefaultAsync(cancellationToken);
        
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        payment.ReferenceTransactionId = request.vnp_TransactionNo;
        
        // Verify payment
        if (!request.IsSuccess)
        {
            payment.Status = PaymentStatus.Failed;
            await _unitOfWork.PaymentRepository.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            return;
        }

        // Update payment status
        payment.Status = PaymentStatus.Completed;
        await _unitOfWork.PaymentRepository.UpdateAsync(payment);
        
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
        }, cancellationToken);
        
        _logger.LogInformation("Completed order {0}.", order.Id);
    }
}