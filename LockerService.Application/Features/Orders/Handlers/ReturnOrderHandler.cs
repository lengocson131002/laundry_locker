using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Handlers;

public class ReturnOrderHandler : IRequestHandler<ReturnOrderCommand, OrderResponse>
{
    private readonly ILogger<UpdateOrderStatusHandler> _logger;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly IRabbitMqBus _rabbitMqBus;

    private readonly ISettingService _settingService;

    private readonly ICurrentAccountService _currentAccountService;

    public ReturnOrderHandler(ILogger<UpdateOrderStatusHandler> logger, IUnitOfWork unitOfWork, IMapper mapper, IRabbitMqBus rabbitMqBus, ISettingService settingService, ICurrentAccountService currentAccountService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _rabbitMqBus = rabbitMqBus;
        _settingService = settingService;
        _currentAccountService = currentAccountService;
    }

    public async Task<OrderResponse> Handle(ReturnOrderCommand request, CancellationToken cancellationToken)
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

        if (!order.CanUpdateStatus(OrderStatus.Returned))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        var previousStatus = order.Status;
        
        //  Return to locker if not support delivery
        if (!order.DeliverySupported)
        {
            var lockerId = order.LockerId;
            var availableBox = await _unitOfWork.BoxRepository.FindAvailableBox(lockerId);
            if (availableBox == null)
            {
                var exception = new ApiException(ResponseCode.LockerErrorNoAvailableBox);
                await _rabbitMqBus.PublishAsync(new LockerOverloadedEvent()
                {
                    LockerId = lockerId,
                    Time = DateTimeOffset.UtcNow,
                    ErrorCode = exception.ErrorCode,
                    Error = exception.ErrorMessage
                }, cancellationToken);

                throw exception;
            }

            order.ReceiveBox = availableBox;
        }

        order.Status = OrderStatus.Returned;
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        var currentAccount = await _currentAccountService.GetRequiredCurrentAccount(); 
        await _rabbitMqBus.PublishAsync(new OrderReturnedEvent()
        {
            Order = order,
            Staff = currentAccount,
            PreviousStatus = previousStatus,
            Time = DateTimeOffset.UtcNow
        }, cancellationToken);

        _logger.LogInformation("Update order status to {0}", order.Status);
        
        return _mapper.Map<OrderResponse>(order);
    }
}