using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Orders.Models;

namespace LockerService.Application.Features.Orders.Handlers;

public class ProcessOvertimeOrderHandler : IRequestHandler<ProcessOvertimeOrderCommand, OrderDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly IRabbitMqBus _rabbitMqBus;

    private readonly ICurrentAccountService _currentAccountService;

    private readonly ILogger<ProcessOvertimeOrderHandler> _logger;

    public ProcessOvertimeOrderHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IRabbitMqBus rabbitMqBus, 
        ICurrentAccountService currentAccountService, 
        ILogger<ProcessOvertimeOrderHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _rabbitMqBus = rabbitMqBus;
        _currentAccountService = currentAccountService;
        _logger = logger;
    }

    public async Task<OrderDetailResponse> Handle(ProcessOvertimeOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == request.OrderId)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }
        
        if (!order.CanUpdateStatus(OrderStatus.OvertimeProcessing))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }
        
        // Save the previous status
        var previousStatus = order.Status;

        // remove receive box
        order.ReceiveBox = null;
        order.Status = OrderStatus.OvertimeProcessing;
        await _unitOfWork.OrderRepository.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        // Push rabbit MQ event
        var currentAccount = await _currentAccountService.GetRequiredCurrentAccount(); 
        await _rabbitMqBus.PublishAsync(new OrderOvertimeProcessingEvent()
        {
            OrderId = order.Id,
            Staff = currentAccount,
            PreviousStatus = previousStatus,
            Time = DateTimeOffset.UtcNow,
            ReceiveBoxNumber = order.ReceiveBox?.Number ?? 0
        }, cancellationToken);

        _logger.LogInformation("Update order status to {0}", order.Status);

        return _mapper.Map<OrderDetailResponse>(order);
    }
}