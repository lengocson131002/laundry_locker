using LockerService.Application.Orders.Queries;

namespace LockerService.Application.Orders.Handlers;

public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, PaginationResponse<Order, OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentAccountService _currentAccountService;

    public GetAllOrdersHandler(IMapper mapper, IUnitOfWork unitOfWork, ICurrentAccountService currentAccountService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _currentAccountService = currentAccountService;
    }

    public async Task<PaginationResponse<Order, OrderResponse>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var currentLoggedInUser = await _currentAccountService.GetCurrentAccount();
        if (currentLoggedInUser != null)
        {
            if (Equals(currentLoggedInUser.Role, Role.Staff))
            {
                /*
                 * Get orders in staff' store
                 */
                request.StoreId = currentLoggedInUser.StoreId;

            } else if (Equals(currentLoggedInUser.Role, Role.Customer))
            {
                /*
                 * Get only customer's orders
                 */
                request.CustomerId = currentLoggedInUser.Id;
            }
        }
        
        var orderQuery = await _unitOfWork.OrderRepository.GetAsync(
            predicate: request.GetExpressions(),
            orderBy: request.GetOrder(),
            includes: new List<Expression<Func<Order, object>>>() {
                order => order.Locker,
                order => order.Staff,
                order => order.SendBox,
                order => order.ReceiveBox,
                order => order.Sender,
                order => order.Receiver
            });

        return new PaginationResponse<Order, OrderResponse>(
            orderQuery, 
            request.PageNumber, 
            request.PageSize, 
            order => _mapper.Map<OrderResponse>(order));
    }
}