using LockerService.Domain.Events;

namespace LockerService.Application.Lockers.Handlers;

public class ConnectLockerHandler : IRequestHandler<ConnectLockerCommand, LockerResponse>
{
    private readonly ILogger<ConnectLockerHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public ConnectLockerHandler(IUnitOfWork unitOfWork, ILogger<ConnectLockerHandler> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<LockerResponse> Handle(ConnectLockerCommand request, CancellationToken cancellationToken)
    {
        // Check locker
        var lockerQuery =
            await _unitOfWork.LockerRepository.GetAsync(
                includes: new List<Expression<Func<Locker, object>>>
                {
                    lo => lo.Location,
                    lo => lo.Location.Province,
                    lo => lo.Location.District,
                    lo => lo.Location.Ward
                },
                disableTracking: false);

        var locker = lockerQuery.FirstOrDefault();
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        var connectEvent = new LockerTimeline()
        {
            Locker = locker,
            Event = LockerEvent.Connect,
            Status = locker.Status
        };
        await _unitOfWork.LockerTimelineRepository.AddAsync(connectEvent);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Locker {0} connected to server", locker.Id);

        return _mapper.Map<LockerResponse>(locker);
    }
}