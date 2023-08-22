using LockerService.Application.Notifications.Commands;
using LockerService.Application.Notifications.Models;

namespace LockerService.Application.Notifications.Handlers;

public class RemoveNotificationHandler : IRequestHandler<RemoveNotificationCommand, NotificationModel>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly ICurrentPrincipalService _currentPrincipalService;

    public RemoveNotificationHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentPrincipalService currentPrincipalService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentPrincipalService = currentPrincipalService;
    }

    public async Task<NotificationModel> Handle(RemoveNotificationCommand request, CancellationToken cancellationToken)
    {
        var currentAccountId = _currentPrincipalService.CurrentSubjectId;
        if (currentAccountId == null)
        {
            throw new ApiException(ResponseCode.Unauthorized);
        }

        var notification = await _unitOfWork.NotificationRepository.GetNotification(currentAccountId.Value, request.Id);

        if (notification == null)
        {
            throw new ApiException(ResponseCode.NotificationErrorNotFound);
        }
        
        notification.DeletedAt = DateTimeOffset.UtcNow;
        await _unitOfWork.NotificationRepository.UpdateAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NotificationModel>(notification);
    }
}