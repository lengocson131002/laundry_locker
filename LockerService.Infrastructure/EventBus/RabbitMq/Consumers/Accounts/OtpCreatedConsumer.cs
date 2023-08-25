using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.EventBus.RabbitMq.Events.Accounts;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Accounts;

public class OtpCreatedConsumer : IConsumer<OtpCreatedEvent>
{
    private readonly INotifier _notifier;
    private readonly IUnitOfWork _unitOfWork;

    public OtpCreatedConsumer(INotifier notifier, IUnitOfWork unitOfWork)
    {
        _notifier = notifier;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<OtpCreatedEvent> context)
    {
        var eventMessage = context.Message;
        var account = await _unitOfWork.AccountRepository.GetCustomerById(eventMessage.AccountId);
        if (account == null)
        {
            return;
        }

        var notification = new Notification()
        {
            Account = account,
            Content = eventMessage.Otp,
            Data = eventMessage.Otp,
            Type = NotificationType.AccountOtpCreated,
            EntityType = EntityType.Account,
            Saved = false
        };

        await _notifier.NotifyAsync(notification);
    }
}