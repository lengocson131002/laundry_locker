using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Services.Payments;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Payments.Models;
using LockerService.Domain;
using LockerService.Shared.Constants;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Orders.Handlers;

public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, PaymentResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRabbitMqBus _rabbitMqBus;
    private readonly IPaymentService _paymentService;

    public CheckoutOrderHandler(IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IRabbitMqBus rabbitMqBus, 
        IPaymentService paymentService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _rabbitMqBus = rabbitMqBus;
        _paymentService = paymentService;
    }

    public async Task<PaymentResponse> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == request.OrderId)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
                .ThenInclude(sender => sender.Wallet)
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

        if (!order.CanUpdateStatus(OrderStatus.Completed))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        // Create payment
        var totalPrice = order.CalculateTotalPrice();
        var prepaidPrice = order.ReservationFee;

        var wallet = order.Sender.Wallet;
        if (wallet == null)
        {
            throw new ApiException(ResponseCode.WalletErrorInvalidBalance);
        }
        
        var chargedPrice = totalPrice > prepaidPrice ? totalPrice - prepaidPrice : 0;
        if (totalPrice > prepaidPrice)
        {
            if (wallet.Balance < chargedPrice)
            {
                throw new ApiException(ResponseCode.WalletErrorInvalidBalance);
            }
            wallet.Balance -= chargedPrice;
            
        }
        else
        {
            wallet.Balance += prepaidPrice - totalPrice;
        }
       
        var payment = new Payment();
        payment.Status = PaymentStatus.Completed;
        payment.Amount = chargedPrice;
        payment.Type = PaymentType.Checkout;
        payment.Content = PaymentType.Checkout.GetDescription();
        payment.Customer = order.Sender;
        payment.Method = PaymentMethod.Wallet;
        await _unitOfWork.PaymentRepository.AddAsync(payment);
        
        // Update sender's wallet
        await _unitOfWork.WalletRepository.UpdateAsync(wallet);
        
        // Update order information
        var prevStatus = order.Status;
        order.Status = OrderStatus.Completed;
        order.ReceiveAt = DateTimeOffset.UtcNow;
        order.TotalPrice = order.CalculateTotalPrice();
        await _unitOfWork.OrderRepository.UpdateAsync(order);

        await _rabbitMqBus.PublishAsync(new OrderCompletedEvent()
        {
            Order = order,
            PreviousStatus = prevStatus,
            Time = DateTimeOffset.UtcNow
        }, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        
        
        return _mapper.Map<PaymentResponse>(payment);
    }
    
}