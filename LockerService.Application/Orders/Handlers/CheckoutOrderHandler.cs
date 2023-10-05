using LockerService.Application.Payments.Models;

namespace LockerService.Application.Orders.Handlers;

public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, PaymentResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;

    public CheckoutOrderHandler(IUnitOfWork unitOfWork, IMapper mapper, IOrderService orderService, IPaymentService paymentService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _orderService = orderService;
        _paymentService = paymentService;
    }

    public async Task<PaymentResponse> Handle(CheckoutOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == command.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
        
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.CanUpdateStatus(OrderStatus.Completed))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }
        
        var payment = await _paymentService.Pay(order, command.Method);
        
        return _mapper.Map<PaymentResponse>(payment);
    }
}