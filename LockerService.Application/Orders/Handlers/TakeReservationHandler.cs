namespace LockerService.Application.Orders.Handlers;

public class TakeReservationHandler : IRequestHandler<TakeReservationCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TakeReservationHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderResponse> Handle(TakeReservationCommand request, CancellationToken cancellationToken)
    {
        var orderQuery = await _unitOfWork.OrderRepository.GetAsync(
            predicate: order => request.PinCode.Equals(order.PinCode) && OrderStatus.Initialized.Equals(order.Status),
            includes: new List<Expression<Func<Order, object>>>
            {
                order => order.Service,
                order => order.Locker
            });

        var order = orderQuery.FirstOrDefault();
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }
        
        var currentStatus = order.Status;
   
        // regenerate order pin code
        order.PinCode = await _unitOfWork.OrderRepository.GenerateOrderPinCode();
        order.PinCodeIssuedAt = DateTimeOffset.UtcNow;
        order.Status = OrderStatus.Waiting;
        
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        
        // Save timeline
        var timeline = new OrderTimeline()
        {
            Order = order,
            PreviousStatus = currentStatus,
            Status = order.Status,
            Time = DateTimeOffset.UtcNow
        };
        
        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OrderResponse>(order);
    }
}