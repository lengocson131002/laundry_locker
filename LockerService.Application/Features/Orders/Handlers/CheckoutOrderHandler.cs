using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Services.Payments;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Payments.Models;
using LockerService.Domain;

namespace LockerService.Application.Features.Orders.Handlers;

public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, PaymentResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRabbitMqBus _rabbitMqBus;
    private readonly IMomoPaymentService _momoPaymentService;
    private readonly IVnPayPaymentService _vnPayPaymentService;

    public CheckoutOrderHandler(IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IRabbitMqBus rabbitMqBus, 
        IMomoPaymentService momoPaymentService, 
        IVnPayPaymentService vnPayPaymentService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _rabbitMqBus = rabbitMqBus;
        _momoPaymentService = momoPaymentService;
        _vnPayPaymentService = vnPayPaymentService;
    }

    public async Task<PaymentResponse> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == request.OrderId)
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

        if (!order.CanUpdateStatus(OrderStatus.Completed))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        Payment payment = null;
        
        if (Equals(request.Method, PaymentMethod.Cash))
        {
            payment = await HandleCashCheckout(order, cancellationToken);
        } else if (Equals(request.Method, PaymentMethod.Momo))
        {
            payment = await HandleMomoCheckout(order, cancellationToken);
        } else if (Equals(request.Method, PaymentMethod.VnPay))
        {
            payment = await HandleVnPayCheckout(order, cancellationToken);
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<PaymentResponse>(payment);
    }

    private async Task<Payment> HandleVnPayCheckout(Order order, CancellationToken cancellationToken)
    {
        var amount = (long)(order.CalculateTotalPrice() - order.ReservationFee);
        var payment = await _vnPayPaymentService.CreatePayment(new VnPayPayment()
        {
            Amount = amount,
            OrderReferenceId = order.ReferenceId ?? throw new Exception("Order referenceId is required"),
            PaymentReferenceId = Guid.NewGuid().ToString(),
            Info = Payment.PaymentContent(order.Type),
            Time = DateTimeOffset.UtcNow
        });

        payment.OrderId = order.Id;
        payment.CustomerId = order.SenderId;
        
        await _unitOfWork.PaymentRepository.AddAsync(payment);
        
        return payment;
    }

    private async Task<Payment> HandleMomoCheckout(Order order, CancellationToken cancellationToken)
    {
        var amount = (long) (order.CalculateTotalPrice() - order.ReservationFee);
        
        var payment = await _momoPaymentService.CreatePayment(new MomoPayment()
        {
            Amount = amount,
            OrderReferenceId = order.ReferenceId ?? throw new Exception("Order referenceId is required"),
            PaymentReferenceId = Guid.NewGuid().ToString(),
            Info =  Payment.PaymentContent(order.Type)
        });

        payment.OrderId = order.Id;
        payment.CustomerId = order.ReceiverId ?? order.SenderId;
        
        await _unitOfWork.PaymentRepository.AddAsync(payment);
        
        return payment;
    }

    private async Task<Payment> HandleCashCheckout(Order order, CancellationToken cancellationToken)
    {
        var payment = new Payment(order, PaymentMethod.Cash, PaymentStatus.Completed);
        await _unitOfWork.PaymentRepository.AddAsync(payment);

        var prevStatus = order.Status;
        order.Status = OrderStatus.Completed;
        order.CompletedAt = DateTimeOffset.UtcNow;
        order.TotalPrice = order.CalculateTotalPrice();
            
        await _unitOfWork.OrderRepository.UpdateAsync(order);
   
        await _rabbitMqBus.PublishAsync(new OrderCompletedEvent()
        {
            Order = order,
            PreviousStatus = prevStatus,
            Time = DateTimeOffset.UtcNow
        }, cancellationToken);

        return payment;
    }
}