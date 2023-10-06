using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Notifications.Commands;
using LockerService.Application.Features.Notifications.Models;

namespace LockerService.Application.Features.Notifications.Handlers;

public class UpdateNotificationStatusHandler : IRequestHandler<UpdateNotificationStatusCommand, NotificationModel>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly ICurrentPrincipalService _currentPrincipalService;

    public UpdateNotificationStatusHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentPrincipalService currentPrincipalService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentPrincipalService = currentPrincipalService;
    }

    public async Task<NotificationModel> Handle(UpdateNotificationStatusCommand request, CancellationToken cancellationToken)
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

        if (notification.IsRead == request.IsRead)
        {
            throw new ApiException(ResponseCode.NotificationErrorInvalidStatus);
        }

        notification.ReadAt = request.IsRead != null && request.IsRead.Value ? DateTimeOffset.UtcNow : null;
        await _unitOfWork.NotificationRepository.UpdateAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NotificationModel>(notification);

    }
}