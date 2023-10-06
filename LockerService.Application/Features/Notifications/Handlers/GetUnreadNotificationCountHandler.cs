using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Notifications.Models;
using LockerService.Application.Features.Notifications.Queries;

namespace LockerService.Application.Features.Notifications.Handlers;

public class GetUnreadNotificationCountHandler : IRequestHandler<GetUnreadNotificationCountQuery, UnreadNotificationCountResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly ICurrentPrincipalService _currentPrincipalService; 

    public GetUnreadNotificationCountHandler(IUnitOfWork unitOfWork, ICurrentPrincipalService currentPrincipalService)
    {
        _unitOfWork = unitOfWork;
        _currentPrincipalService = currentPrincipalService;
    }

    public async Task<UnreadNotificationCountResponse> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
    {
        var loggedInAccountId = _currentPrincipalService.CurrentSubjectId;
        if (loggedInAccountId == null)
        {
            throw new ApiException(ResponseCode.Unauthorized);
        }
        
        var count = await _unitOfWork.NotificationRepository
            .Get(notification => !notification.IsRead
                                 && notification.AccountId == loggedInAccountId
                                 && (request.Type == null || Equals(notification.Type, request.Type))
                                 && (request.EntityType == null || Equals(notification.EntityType, request.EntityType))
                                 && (request.From == null || Equals(notification.CreatedAt >= request.From))
                                 && (request.To == null || Equals(notification.CreatedAt <= request.To)))
            .CountAsync(cancellationToken);

        return new UnreadNotificationCountResponse(count);

    }
}