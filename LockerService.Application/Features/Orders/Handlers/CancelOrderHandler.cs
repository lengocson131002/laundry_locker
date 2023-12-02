using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Handlers;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly ICurrentAccountService _currentAccountService;

    private readonly IMapper _mapper;

    private readonly IRabbitMqBus _rabbitMqBus;

    public CancelOrderHandler(IUnitOfWork unitOfWork,
     ICurrentAccountService currentAccountService,
      IMapper mapper,
       IRabbitMqBus rabbitMqBus)
    {
        _unitOfWork = unitOfWork;
        _currentAccountService = currentAccountService;
        _mapper = mapper;
        _rabbitMqBus = rabbitMqBus;
    }


    public async Task<OrderResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == request.OrderId)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Locker.Location)
            .Include(order => order.Locker.Location.Ward)
            .Include(order => order.Locker.Location.District)
            .Include(order => order.Locker.Location.Province)
            .Include(order => order.Details)
            .ThenInclude(detail => detail.Service)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.CanUpdateStatus(OrderStatus.Canceled))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        var previousStatus = order.Status;
        
        order.Status = OrderStatus.Canceled;
        order.ReceiveAt = DateTimeOffset.UtcNow;
        order.TotalPrice = order.ReservationFee;
        
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        await _rabbitMqBus.PublishAsync(new OrderCanceledEvent()
        {
            Order = order,
            PreviousStatus = previousStatus,
            Time = DateTimeOffset.UtcNow
        }, cancellationToken);

        return _mapper.Map<OrderResponse>(order);
    }
}