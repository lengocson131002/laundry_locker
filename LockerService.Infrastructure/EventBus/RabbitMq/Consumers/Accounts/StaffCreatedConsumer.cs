using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.EventBus.RabbitMq.Events.Accounts;
using LockerService.Domain.Enums;

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
        
        var notification = new Notification()
        {
            Account = staffAccount,
            Type = NotificationType.AccountStaffCreated,
            EntityType = EntityType.Account,
            Content = NotificationType.AccountStaffCreated.GetDescription(),
            Data = string.Empty,
            Saved = false,
            ReferenceId = eventMessage.AccountId.ToString()
        };
        
        await _notifier.NotifyAsync(notification);
    }
}