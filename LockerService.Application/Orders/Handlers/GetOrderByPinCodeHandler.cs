using LockerService.Application.Orders.Queries;

namespace LockerService.Application.Orders.Handlers;

public class GetOrderByPinCodeHandler : IRequestHandler<GetOrderByPinCodeQuery, OrderDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly IOrderService _orderService;

    public GetOrderByPinCodeHandler(IMapper mapper, IUnitOfWork unitOfWork, IOrderService orderService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _orderService = orderService;
    }

    public async Task<OrderDetailResponse> Handle(GetOrderByPinCodeQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetOrderByPinCode(request.PinCode, request.LockerId)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Staff)
            .Include(order => order.Details)
                .ThenInclude(detail => detail.Service)
            .Include(order => order.Details)
                .ThenInclude(detail => detail.Items)
            .Include(order => order.Timelines)
                .ThenInclude(detail => detail.Staff)
            .Include(order => order.Locker.Location)
            .Include(order => order.Locker.Location.Ward)
            .Include(order => order.Locker.Location.District)
            .Include(order => order.Locker.Location.Province)
            .Include(order => order.DeliveryAddress)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        order.Timelines = order.Timelines.OrderByDescending(timeline => timeline.CreatedAt).ToList();

        return _mapper.Map<OrderDetailResponse>(order);
    }
    
}