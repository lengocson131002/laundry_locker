using LockerService.Application.Notifications.Models;
using LockerService.Application.Notifications.Queries;

namespace LockerService.Application.Notifications.Handlers;

public class GetAllNotificationsHandler : IRequestHandler<GetAllNotificationsQuery, PaginationResponse<Notification, NotificationModel>>
{
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly ICurrentPrincipalService _currentPrincipalService;

    private readonly IMapper _mapper;

    public GetAllNotificationsHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentPrincipalService currentPrincipalService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentPrincipalService = currentPrincipalService;
    }

    public async Task<PaginationResponse<Notification, NotificationModel>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        var currentAccountId = _currentPrincipalService.CurrentSubjectId;
        if (currentAccountId == null)
        {
            throw new ApiException(ResponseCode.Unauthorized);
        }

        request.AccountId = currentAccountId;
        request.SortDir = SortDirection.Desc;
        request.SortColumn = "CreatedAt";
        
        var notificationQuery = await _unitOfWork.NotificationRepository.GetAsync(
            predicate: request.GetExpressions(),
            orderBy: request.GetOrder(),
            disableTracking: true);

        return new PaginationResponse<Notification, NotificationModel>(
            notificationQuery,
            request.PageNumber,
            request.PageSize,
            notification => _mapper.Map<NotificationModel>(notification));
    }
}