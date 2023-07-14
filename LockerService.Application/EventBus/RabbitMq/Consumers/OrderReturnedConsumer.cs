using System.Text.Json;
using LockerService.Application.Common.Constants;
using LockerService.Application.Common.Services.Notification;
using LockerService.Application.Common.Services.Notification.Data;
using LockerService.Application.EventBus.RabbitMq.Events;
using MassTransit;

namespace LockerService.Application.EventBus.RabbitMq.Consumers;

public class OrderReturnedConsumer : IConsumer<OrderReturnedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISmsNotificationService _smsNotificationService;

    public OrderReturnedConsumer(
        ILogger<OrderCreatedConsumer> logger, 
        IUnitOfWork unitOfWork, 
        ISmsNotificationService smsNotificationService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _smsNotificationService = smsNotificationService;
    }
    
    public async Task Consume(ConsumeContext<OrderReturnedEvent> context)
    {
        var eventMessage = context.Message;
        var orderQuery = await _unitOfWork.OrderRepository.GetAsync(
            predicate: order => order.Id == eventMessage.Id,
            includes: new ListResponse<Expression<Func<Order, object>>>()
            {
                order => order.Locker,
                order => order.Locker.Location,
                order => order.Locker.Location.Ward,
                order => order.Locker.Location.District,
                order => order.Locker.Location.Province
            });
        
        var order = await orderQuery.FirstOrDefaultAsync();

        if (order == null)
        {
            return;
        }

        var locker = order.Locker;
        var address = $"{locker.Location.Address}, {locker.Location.Ward.Name}, {locker.Location.District.Name}, {locker.Location.Province.Name}";
        
        var smsContent = string.Format(SmsTemplates.OrderReturnedSmsTemplate, order.PinCode);
        var notifiedPhone = !string.IsNullOrWhiteSpace(order.ReceivePhone) ? order.ReceivePhone : order.OrderPhone;
        var smsData = new SmsNotificationData(notifiedPhone, smsContent);
        
        _logger.LogInformation("Send sms: {0}", JsonSerializer.Serialize(smsData));
        
        await _smsNotificationService.SendAsync(smsData);
    }
}