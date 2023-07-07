using LockerService.Application.Orders.Queries;

namespace LockerService.Application.Orders.Handlers;

public class GetOrderDetailHandler : IRequestHandler<GetOrderDetailQuery, OrderItemResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderDetailHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderItemResponse> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(request.DetailId);
        if (orderDetail == null || orderDetail.OrderId != order.Id)
        {
            throw new ApiException(ResponseCode.OrderErrorDetailNotFound);
        }

        return _mapper.Map<OrderItemResponse>(order);
    }
}