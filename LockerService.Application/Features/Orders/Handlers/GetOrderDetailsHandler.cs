using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Orders.Models;
using LockerService.Application.Features.Orders.Queries;

namespace LockerService.Application.Features.Orders.Handlers;

public class GetOrderDetailsHandler : IRequestHandler<GetOrderDetailsQuery, ListResponse<OrderItemResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public GetOrderDetailsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ListResponse<OrderItemResponse>> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var orderDetails = await _unitOfWork.OrderDetailRepository
            .Get(detail => detail.OrderId == request.OrderId)
            .Include(detail => detail.Service)
            .Include(detail => detail.Items)
            .ToListAsync(cancellationToken);

        var orderDetailsResponse = _mapper.Map<List<OrderItemResponse>>(orderDetails);

        return new ListResponse<OrderItemResponse>(orderDetailsResponse);
    }
}