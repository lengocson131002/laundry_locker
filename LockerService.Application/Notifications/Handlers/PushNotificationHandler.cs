using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Notifications.Commands;

namespace LockerService.Application.Notifications.Handlers;

public class PushNotificationHandler : IRequestHandler<PushNotificationCommand>
{
    private readonly INotifier _notifier;
    private readonly ICurrentAccountService _currentAccountService;

    public PushNotificationHandler(INotifier notifier, ICurrentAccountService currentAccountService)
    {
        _notifier = notifier;
        _currentAccountService = currentAccountService;
    }

    public async Task Handle(PushNotificationCommand request, CancellationToken cancellationToken)
    {
        var currentAcc = await _currentAccountService.GetCurrentAccount();
        if (currentAcc == null)
        {
            throw new ApiException(ResponseCode.Unauthorized);
        }
        var notification = new Notification()
        {
            Type = request.Type,
            Content = "Test notification",
            EntityType = EntityType.Account,
            Account = currentAcc,
            Data = "Test data content"
        };

        await _notifier.NotifyAsync(notification);
    }
}