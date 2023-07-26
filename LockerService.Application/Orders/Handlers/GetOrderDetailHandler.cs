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

        var orderDetail = await _unitOfWork.OrderDetailRepository.Get(
            predicate: detail => detail.Id == request.DetailId && detail.OrderId == request.OrderId,
            includes: new List<Expression<Func<OrderDetail, object>>>()
            {
                detail => detail.Service
            }
        ).FirstOrDefaultAsync(cancellationToken);
        
        if (orderDetail == null)
        {
            throw new ApiException(ResponseCode.OrderDetailErrorInfoRequired);
        }

        return _mapper.Map<OrderItemResponse>(orderDetail);
    }
}