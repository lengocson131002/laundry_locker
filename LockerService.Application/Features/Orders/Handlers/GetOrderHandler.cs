using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Orders.Models;
using LockerService.Application.Features.Orders.Queries;

namespace LockerService.Application.Features.Orders.Handlers;

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
            disableTracking: true);
        var order = await orderQuery
            .Include(order => order.Locker)
            .Include((order => order.Locker.Store))
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
            .Include(order => order.DeliveryAddress.Ward)
            .Include(order => order.DeliveryAddress.District)
            .Include(order => order.DeliveryAddress.Province)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }
        
        order.Timelines = order.Timelines.OrderByDescending(timeline => timeline.CreatedAt).ToList();
        
        return _mapper.Map<OrderDetailResponse>(order);
    }
    
} 