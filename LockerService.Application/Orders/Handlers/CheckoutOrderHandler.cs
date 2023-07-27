using LockerService.Application.Bills.Models;
using Quartz;

namespace LockerService.Application.Orders.Handlers;

public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, BillResponse>
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

    public async Task<BillResponse> Handle(CheckoutOrderCommand command, CancellationToken cancellationToken)
    {
        var orderQuery = await _unitOfWork.OrderRepository.GetAsync(
            predicate: order => order.Id == command.Id,
            includes: new List<Expression<Func<Order, object>>>()
            {
                order => order.Details
            });

        var order = await orderQuery.FirstOrDefaultAsync(cancellationToken);
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.CanCheckout)
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        await _orderService.CalculateFree(order);
        
        var bill = Bill.CreateBill(order, command.Method);
        
        /*
         * Test only
         * Set job to complete order in 30s
         */
        await _paymentService.Pay(order, command.Method);
        
        return _mapper.Map<BillResponse>(bill);
    }
}