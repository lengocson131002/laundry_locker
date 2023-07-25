using LockerService.Application.Orders.Queries;

namespace LockerService.Application.Orders.Handlers;

public class GetOrderByPinCodeHandler : IRequestHandler<GetOrderByPinCodeQuery, OrderDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public GetOrderByPinCodeHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDetailResponse> Handle(GetOrderByPinCodeQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetOrderByPinCode(request.PinCode)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Bill)
            .Include(order => order.Details)
            .Include(order => order.Timelines)
            .Include(order => order.Locker.Location)
            .Include(order => order.Locker.Location.Ward)
            .Include(order => order.Locker.Location.District)
            .Include(order => order.Locker.Location.Province)
            .FirstOrDefaultAsync(cancellationToken);

        
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }
        
        order.Timelines = order.Timelines.OrderBy(timeline => timeline.CreatedAt).ToList();
        return _mapper.Map<OrderDetailResponse>(order);
    }
    
}