using AutoMapper;
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Entities.Settings;
using LockerService.Domain.Enums;
using LockerService.Shared.Extensions;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Orders;

public class OrderUpdatedStatusConsumer : IConsumer<OrderUpdatedStatusEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<OrderUpdatedStatusConsumer> _logger;
    
    private readonly IMapper _mapper;

    private readonly ISettingService _settingService;

    private readonly IOrderService _orderService;

    private readonly IMqttBus _mqttBus;
    
    private readonly INotifier _notifier;

    public OrderUpdatedStatusConsumer(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        ILogger<OrderUpdatedStatusConsumer> logger, 
        ISettingService settingService, 
        IOrderService orderService, IMqttBus mqttBus, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _settingService = settingService;
        _orderService = orderService;
        _mqttBus = mqttBus;
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<OrderUpdatedStatusEvent> context)
    {
        var eventMessage = context.Message;
        
        _logger.LogInformation("Received order updated status message: {0}", JsonSerializer.Serialize(eventMessage));

        var order = await _unitOfWork.OrderRepository.Get(order => order.Id == eventMessage.OrderId)
            .Include(order => order.Locker)
            .Include(order => order.SendBox)
            .Include(order => order.ReceiveBox)
            .Include(order => order.Sender)
            .Include(order => order.Receiver)
            .Include(order => order.Details)
            .ThenInclude(detail => detail.Service)
            .Include(order => order.Locker.Location)
            .Include(order => order.Locker.Location.Ward)
            .Include(order => order.Locker.Location.District)
            .Include(order => order.Locker.Location.Province)
            .FirstOrDefaultAsync();
        
        if (order == null)
        {
            return;
        }
        
        // Handle update status
        switch (eventMessage.Status)
        {
            case OrderStatus.Initialized:
                await HandleOrderInitialized(order, eventMessage.Time);
                break;
            
            case OrderStatus.Waiting:
                await HandleOrderConfirmed(order, eventMessage.Time);
                break;
            
            case OrderStatus.Collected:
                await HandleOrderCollected(order, eventMessage.Time);
                break;
            
            case OrderStatus.Processed:
                await HandleOrderProcessed(order, eventMessage.Time);
                break;
            
            case OrderStatus.Returned:
                await HandleOrderReturned(order, eventMessage.Time);
                break;
            
            case OrderStatus.Completed:
                await HandlerOrderCompleted(order, eventMessage.Time);
                break;
            
            case OrderStatus.Canceled:
                await HandleOrderCanceled(order, eventMessage.Time);
                break;
            
            case OrderStatus.Overtime:
                await HandlerOrderOvertime(order, eventMessage.Time);
                break;
            
            case OrderStatus.Reserved:
                await HandleOrderReserved(order, eventMessage.Time);
                break;
        }
        
        // Save timeline
        var timeline = new OrderTimeline()
        {
            OrderId = order.Id,
            PreviousStatus = eventMessage.PreviousStatus,
            Status = eventMessage.Status,
            Image = eventMessage.Image,
            Description = eventMessage.Description,
            StaffId = eventMessage.StaffId
        };

        await _unitOfWork.OrderTimelineRepository.AddAsync(timeline);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandleOrderReserved(Order order, DateTimeOffset eventMessageTime)
    {
        var orderSettings = await _settingService.GetSettings<OrderSettings>();
        var cancelTime = order.CreatedAt.AddMinutes(orderSettings.ReservationInitTimeoutInMinutes);
        await _orderService.CancelExpiredOrder(order.Id, cancelTime);
    }

    private async Task HandlerOrderOvertime(Order order, DateTimeOffset eventMessageTime)
    {
        
        _logger.LogInformation("Handle order overtime event");
        
        await _notifier.NotifyAsync(
            new Notification(
                account: order.Sender,
                type: NotificationType.CustomerOrderOverTime,
                entityType: EntityType.Order,
                data: order
            ));
        
        if (order.ReceiverId != null && order.Receiver != null)
        {
            await _notifier.NotifyAsync(new Notification(
                account: order.Receiver,
                type: NotificationType.CustomerOrderOverTime,
                entityType: EntityType.Order,
                data: order
            ));
        }
        
        // Notify manager to handle this order
        var staffs = await _unitOfWork.AccountRepository
            .GetStaffs(storeId: order.Locker.StoreId)
            .ToListAsync();;
        
        foreach (var staff in staffs)
        {
            var notification = new Notification(
                account: staff,
                type: NotificationType.SystemOrderOverTime,
                entityType: EntityType.Locker,
                data: order
            );
            
            await _notifier.NotifyAsync(notification);
        }

    }

    private async Task HandleOrderCanceled(Order order, DateTimeOffset eventMessageTime)
    {
        await _notifier.NotifyAsync(new Notification(
            account: order.Sender,
            type: NotificationType.CustomerOrderCanceled,
            entityType: EntityType.Order,
            data: order
        ));
    }

    private async Task HandlerOrderCompleted(Order order, DateTimeOffset eventMessageTime)
    {
        _logger.LogInformation("Handle order completed event");
        
        // Push MQTT to open box
        if (order.ReceiveBox != null && order.ReceiveBoxId != null)
        {
            await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
            {
                LockerCode = order.Locker.Code,
                BoxNumber = order.ReceiveBox.Number
            });
        }
        
        // Push notification
        await _notifier.NotifyAsync(
            new Notification(
                account: order.Sender,
                type: NotificationType.CustomerOrderCompleted,
                entityType: EntityType.Order,
                data: order
            ));
        
        if (order.Receiver != null)
        {
            await _notifier.NotifyAsync(new Notification(
                account: order.Receiver,
                type: NotificationType.CustomerOrderCompleted,
                entityType: EntityType.Order,
                data: order
            ));
        }
    }

    private async Task HandleOrderReturned(Order order, DateTimeOffset eventMessageTime)
    {
        _logger.LogInformation("Handle order returned event");
        
        // Mqtt Open Box
        if (order.ReceiveBox != null && order.ReceiveBoxId != null)
        {
            await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
            {
                LockerCode = order.Locker.Code,
                BoxNumber = order.ReceiveBox.Number
            });
        }
        
        // Push notification
        var notiAccount = order.ReceiverId != null && order.Receiver != null ? order.Receiver : order.Sender;
        await _notifier.NotifyAsync(
            new Notification(
                account: notiAccount,
                type: NotificationType.CustomerOrderReturned,
                entityType: EntityType.Order,
                data: order));
        
        // Check locker box availability
        await CheckLockerBoxAvailability(order.Locker);
    }

    private Task HandleOrderProcessed(Order order, DateTimeOffset time)
    {
        if (!order.IsLaundry)
        {
            return Task.CompletedTask;
        }
        
        _logger.LogInformation("Handle order process event");
        return Task.CompletedTask;
    }

    private async Task HandleOrderCollected(Order order, DateTimeOffset time)
    {
        if (!order.IsLaundry)
        {
            return;
        }
        
        _logger.LogInformation("Handle order collected event");
        
        // Push MQTT to open box
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
        {
            LockerCode = order.Locker.Code,
            BoxNumber = order.SendBox.Number
        });
    }

    private async Task HandleOrderInitialized(Order order, DateTimeOffset time)
    {
        _logger.LogInformation("Handle order initialized event");
        var orderSettings = await _settingService.GetSettings<OrderSettings>();
        var cancelTime = time.AddMinutes(orderSettings.InitTimeoutInMinutes);

        await _orderService.CancelExpiredOrder(order.Id, cancelTime);
        
        // Push MQTT to open box
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
        {
            LockerCode = order.Locker.Code,
            BoxNumber = order.SendBox.Number
        });
    }
    
    private async Task HandleOrderConfirmed(Order order, DateTimeOffset eventMessageTime)
    {
        _logger.LogInformation("Handle order confirm event");
        
        await _notifier.NotifyAsync(
            new Notification(
                account: order.Sender,
                type: NotificationType.CustomerOrderCreated,
                entityType: EntityType.Order,
                data: order
            ));

        if (order.ReceiverId != null && order.Receiver != null)
        {
            await _notifier.NotifyAsync(
                new Notification(
                    account: order.Receiver,
                    type: NotificationType.CustomerOrderCreated,
                    entityType: EntityType.Order,
                    data: order
                ));
        }
        
        // Notify for staff to collect when order type is laundry
        if (order.IsLaundry)
        {
            var staffs = await _unitOfWork.AccountRepository
                .GetStaffs(storeId: order.Locker.StoreId)
                .ToListAsync();
            
            foreach (var staff in staffs)
            {
                var notification = new Notification(
                    account: staff,
                    type: NotificationType.SystemOrderCreated,
                    entityType: EntityType.Order,
                    data: order
                );
            
                await _notifier.NotifyAsync(notification);
            }     
        }
        
        // Check locker box availability
        await CheckLockerBoxAvailability(order.Locker);
    }

    private async Task CheckLockerBoxAvailability(Locker locker)
    {
        var lockerSettings = await _settingService.GetSettings<LockerSettings>();
        
        var availableBoxes = await _unitOfWork.BoxRepository.FindAvailableBoxes(locker.Id);

        if (availableBoxes.Count <= lockerSettings.AvailableBoxCountWarning)
        {
            var laundryAttendants =await _unitOfWork.AccountRepository
                .GetStaffs(storeId: locker.StoreId)
                .ToListAsync();
            
            foreach (var la in laundryAttendants)
            {
                var notification = new Notification(
                    account: la,
                    type: NotificationType.SystemLockerBoxWarning,
                    entityType: EntityType.Locker,
                    data: locker
                );

                await _notifier.NotifyAsync(notification);
            }
        }
    }
}