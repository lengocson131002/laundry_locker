using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.Features.Payments.Commands;
using LockerService.Domain;

namespace LockerService.Application.Features.Payments.Handlers;

public class PaymentCallbackHandler : IRequestHandler<PaymentCallbackCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IRabbitMqBus _rabbitMqBus;

    private readonly ILogger<PaymentCallbackHandler> _logger;

    public PaymentCallbackHandler(IUnitOfWork unitOfWork, IRabbitMqBus rabbitMqBus, ILogger<PaymentCallbackHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
        _logger = logger;
    }

    public async Task Handle(PaymentCallbackCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.PaymentRepository
            .Get(payment => payment.ReferenceId == request.PaymentReferenceId 
                            && Equals(payment.Status, PaymentStatus.Created))
            .FirstOrDefaultAsync(cancellationToken);

        if (payment == null)
        {
            throw new ApiException(ResponseCode.PaymentErrorNotFound);
        }

        // Verify payment
        if (!request.IsSuccess)
        {
            payment.Status = PaymentStatus.Failed;
            await _unitOfWork.PaymentRepository.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            return;
        }

        // Handle payment callback
        await HandlePaymentCallback(payment, request);
        
        // Update payment status
        payment.ReferenceTransactionId = request.ReferenceTransactionId;

        // Update payment status
        payment.Status = PaymentStatus.Completed;
        await _unitOfWork.PaymentRepository.UpdateAsync(payment);
        
        // Save all changes
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Payment callback from {0}. IsSuccess: {1}, Amount: {2}", payment.Method, request.IsSuccess, request.Amount);
    }

    private async Task HandlePaymentCallback(Payment payment, PaymentCallbackCommand request)
    {
        switch (payment.Type)
        {
            case PaymentType.Deposit:
                await HandleDepositPaymentCallback(payment, request);
                break;
            case PaymentType.Checkout:
                await HandleDepositCheckoutCallback(payment, request);
                break;
        }
    }

    private async Task HandleDepositCheckoutCallback(Payment payment, PaymentCallbackCommand request)
    {
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
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        // Update order status
        var currStatus = order.Status;
        order.Status = OrderStatus.Completed;
        order.ReceiveAt = DateTimeOffset.UtcNow;
        order.TotalPrice = order.CalculateTotalPrice();

        await _unitOfWork.OrderRepository.UpdateAsync(order);
        
        // Push event
        await _rabbitMqBus.PublishAsync(new OrderCompletedEvent()
        {
            Order = order,
            PreviousStatus = currStatus,
            Time = DateTimeOffset.UtcNow
        });
    }

    private async Task HandleDepositPaymentCallback(Payment payment, PaymentCallbackCommand request)
    {
        // get customer
        var customer = await _unitOfWork.AccountRepository
            .Get(acc => acc.Id == payment.CustomerId)
            .Include(acc => acc.Wallet)
            .FirstOrDefaultAsync();

        if (customer == null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        var wallet = customer.Wallet;
        if (wallet == null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        wallet.Balance += payment.Amount;
        wallet.LastDepositAt = DateTimeOffset.UtcNow;
        
        // Push notifications
        
        await _unitOfWork.WalletRepository.UpdateAsync(wallet);

    }
}