using LockerService.Application.Orders.Queries;

namespace LockerService.Application.Orders.Handlers;

public class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public GetOrderHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDetailResponse> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var orderQuery = await _unitOfWork.OrderRepository.GetAsync(
            predicate: order => order.Id == request.Id,
            includes: new List<Expression<Func<Order, object>>>
            {
                order => order.Locker,
                order => order.Sender,
                order => order.Receiver,
                order => order.SendBox,
                order => order.ReceiveBox,
                order => order.Bill,
                order => order.Timelines,
                order => order.Locker.Location,
                order => order.Locker.Location.Ward,
                order => order.Locker.Location.District,
                order => order.Locker.Location.Province,
                order => order.Details,
                order => order.Timelines
            });
        
        var order = orderQuery.FirstOrDefault();
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        order.Timelines = order.Timelines.OrderBy(timeline => timeline.CreatedAt).ToList();
        return _mapper.Map<OrderDetailResponse>(order);
    }
    
}