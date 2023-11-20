using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Services.Payments;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Payments.Models;
using LockerService.Shared.Constants;

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

        var payment = await _paymentService.Checkout(order, request.Method, cancellationToken);

        if (payment.Completed)
        {
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
        }

        // Save changes
        await _unitOfWork.PaymentRepository.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        if (payment.Created)
        {
            await _paymentService.SetPaymentTimeOut(payment, DateTimeOffset.UtcNow.AddMinutes(PaymentConstants.PaymentTimeoutInMinutes));
        }
        
        return _mapper.Map<PaymentResponse>(payment);
    }
    
}