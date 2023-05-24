using LockerService.Application.Orders.Queries;

namespace LockerService.Application.Orders.Handlers;

public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, PaginationResponse<Order, OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllOrdersHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResponse<Order, OrderResponse>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orderQuery = await _unitOfWork.OrderRepository.GetAsync(
            predicate: request.GetExpressions(),
            orderBy: request.GetOrder(),
            includes: new List<Expression<Func<Order, object>>>() {
                order => order.Locker,
                order => order.Service
            });

        return new PaginationResponse<Order, OrderResponse>(
            orderQuery, 
            request.PageNumber, 
            request.PageSize, 
            order => _mapper.Map<OrderResponse>(order));
    }
}