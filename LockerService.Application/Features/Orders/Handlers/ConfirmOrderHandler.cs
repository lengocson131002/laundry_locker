using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Handlers;

public class ConfirmOrderHandler : IRequestHandler<ConfirmOrderCommand, OrderResponse>
{
    private readonly ILogger<UpdateOrderStatusHandler> _logger;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly IRabbitMqBus _rabbitMqBus;

    private readonly ISettingService _settingService;

    public ConfirmOrderHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IRabbitMqBus rabbitMqBus, 
        ISettingService settingService, 
        ILogger<UpdateOrderStatusHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _rabbitMqBus = rabbitMqBus;
        _settingService = settingService;
        _logger = logger;
    }
    
    public async Task<OrderResponse> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
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

        if (!order.CanUpdateStatus(OrderStatus.Waiting))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }
        
        var previousStatus = order.Status;

        order.Status = OrderStatus.Waiting;
        order.PinCodeIssuedAt = DateTimeOffset.UtcNow;
        order.CreatedAt = DateTimeOffset.UtcNow;
        
        // Update order
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        await _rabbitMqBus.PublishAsync(new OrderConfirmedEvent()
        {
            Order = order,
            PreviousStatus = previousStatus,
            Time = DateTimeOffset.UtcNow,
        }, cancellationToken);

        _logger.LogInformation("Update order status to {0}", order.Status);
        
        return _mapper.Map<OrderResponse>(order);
    }
}