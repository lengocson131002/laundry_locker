using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Orders.Models;
using LockerService.Domain.Entities.Settings;

namespace LockerService.Application.Features.Orders.Handlers;

public class AddMoreItemsHandler : IRequestHandler<AddMoreItemsCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly ISettingService _settingService;
    
    private readonly IMqttBus _mqttBus;

    private readonly IOrderService _orderService;

    public AddMoreItemsHandler(IUnitOfWork unitOfWork, IMapper mapper, IMqttBus mqttBus, ISettingService settingService, IOrderService orderService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mqttBus = mqttBus;
        _settingService = settingService;
        _orderService = orderService;
    }

    public async Task<OrderResponse> Handle(AddMoreItemsCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.OrderRepository
            .Get(order => Equals(order.Id, request.OrderId))
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null)
        {
            throw new ApiException(ResponseCode.OrderErrorNotFound);
        }

        if (!order.CanUpdateStatus(OrderStatus.Updating))
        {
            throw new ApiException(ResponseCode.OrderErrorInvalidStatus);
        }

        var newStatus = OrderStatus.Updating;
        
        // save timeline
        var timeline = new OrderTimeline()
        {
            OrderId = order.Id,
            Status = newStatus,
            PreviousStatus = order.Status
        };
        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        
        // update order status 
        order.Status = newStatus;
        await _unitOfWork.OrderRepository.UpdateAsync(order);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        
        // set timeout for clear order if customer didn't confirm after adding more items 
        var orderSettings = await _settingService.GetSettings<OrderSettings>(cancellationToken);
        var cancelTime = DateTimeOffset.UtcNow.AddMinutes(orderSettings.InitTimeoutInMinutes);
        await _orderService.CancelExpiredOrder(order.Id, cancelTime);
        
        // push MQTT to open box for adding more items
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
        {
            LockerCode = order.Locker.Code,
            BoxNumber = order.SendBox.Number
        });
        
        return _mapper.Map<OrderResponse>(order);
    }
}