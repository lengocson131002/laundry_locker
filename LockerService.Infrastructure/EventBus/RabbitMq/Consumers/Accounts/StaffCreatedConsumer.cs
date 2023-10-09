using LockerService.Application.EventBus.RabbitMq.Events.Accounts;
using LockerService.Domain.Enums;
using LockerService.Shared.Extensions;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Accounts;

public class StaffCreatedConsumer : IConsumer<StaffCreatedEvent>
{
    private readonly INotifier _notifier;

    public StaffCreatedConsumer(INotifier notifier)
    {
        _notifier = notifier;
    }

    public async Task Consume(ConsumeContext<StaffCreatedEvent> context)
    {
        var eventMessage = context.Message;
        var staffAccount = new Account()
        {
            Id = eventMessage.AccountId,
            PhoneNumber = eventMessage.PhoneNumber,
            Username = eventMessage.PhoneNumber,
            Password = eventMessage.Password,
            Role = eventMessage.Role,
            FullName = eventMessage.FullName,
        };
        
        var notification = new Notification(
            account: staffAccount,
            type: NotificationType.SystemStaffCreated,
            entityType: EntityType.Account,
            data: staffAccount,
            saved: false
        );
        
        await _notifier.NotifyAsync(notification);
    }
}