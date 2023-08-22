using LockerService.Application.Notifications.Models;
using LockerService.Application.Notifications.Queries;

namespace LockerService.Application.Notifications.Handlers;

public class GetNotificationHandler : IRequestHandler<GetNotificationQuery, NotificationModel>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ICurrentPrincipalService _currentPrincipalService;

    private readonly IMapper _mapper;
    
    public GetNotificationHandler(IUnitOfWork unitOfWork, ICurrentPrincipalService currentPrincipalService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentPrincipalService = currentPrincipalService;
        _mapper = mapper;
    }

    public async Task<NotificationModel> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
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

        return _mapper.Map<NotificationModel>(notification);
    }
}