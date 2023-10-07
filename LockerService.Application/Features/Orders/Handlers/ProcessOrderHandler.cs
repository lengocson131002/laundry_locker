using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Handlers;

public class ProcessOrderHandler : IRequestHandler<ProcessOrderCommand, OrderResponse>
{
    private readonly ILogger<UpdateOrderStatusHandler> _logger;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly IRabbitMqBus _rabbitMqBus;

    private readonly ISettingService _settingService;

    private readonly ICurrentAccountService _currentAccountService;

    public ProcessOrderHandler(IUnitOfWork unitOfWork, 
        ISettingService settingService, 
        IRabbitMqBus rabbitMqBus, 
        IMapper mapper, 
        ILogger<UpdateOrderStatusHandler> logger, ICurrentAccountService currentAccountService)
    {
        _unitOfWork = unitOfWork;
        _settingService = settingService;
        _rabbitMqBus = rabbitMqBus;
        _mapper = mapper;
        _logger = logger;
        _currentAccountService = currentAccountService;
    }

    public async Task<OrderResponse> Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
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
        
        if (order == null || !order.IsLaundry)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.CanUpdateStatus(OrderStatus.Processed))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        var previousStatus = order.Status;
        
        order.Status = OrderStatus.Processed;
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        var currentAccount = await _currentAccountService.GetRequiredCurrentAccount(); 
        await _rabbitMqBus.PublishAsync(new OrderProcessedEvent()
        {
            PreviousStatus = previousStatus,
            Time = DateTimeOffset.UtcNow,
            Order = order,
            Staff = currentAccount
        }, cancellationToken);

        _logger.LogInformation("Update order status to {0}", order.Status);
        
        return _mapper.Map<OrderResponse>(order);
    }
}