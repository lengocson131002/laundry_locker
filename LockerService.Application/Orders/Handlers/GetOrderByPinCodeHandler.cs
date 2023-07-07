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
        var order = await _unitOfWork.OrderRepository.GetOrderByPinCode(request.PinCode);
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }
        order.Timelines = order.Timelines.OrderBy(timeline => timeline.Time).ToList();
        return _mapper.Map<OrderDetailResponse>(order);
    }
    
}