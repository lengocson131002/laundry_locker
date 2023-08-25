using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Notifications.Commands;

namespace LockerService.Application.Notifications.Handlers;

public class PushNotificationHandler : IRequestHandler<PushNotificationCommand>
{
    private readonly INotifier _notifier;
    private readonly IUnitOfWork _unitOfWork;
    public PushNotificationHandler(INotifier notifier, IUnitOfWork unitOfWork)
    {
        _notifier = notifier;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(PushNotificationCommand request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.AccountRepository.GetByIdAsync(request.AccountId);
        if (account == null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }
        
        var notification = new Notification()
        {
            Type = request.Type,
            Content = "Test notification",
            EntityType = EntityType.Account,
            AccountId = account.Id,
            Account = account,
            Data = request.Data,
            Saved = false
        };

        await _notifier.NotifyAsync(notification);
    }
}