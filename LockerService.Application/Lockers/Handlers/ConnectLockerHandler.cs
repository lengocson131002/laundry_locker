using LockerService.Application.EventBus.RabbitMq.Events.Lockers;

namespace LockerService.Application.Lockers.Handlers;

public class ConnectLockerHandler : IRequestHandler<ConnectLockerCommand, LockerResponse>
{
    private readonly ILogger<ConnectLockerHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRabbitMqBus _rabbitMqBus;
    public ConnectLockerHandler(IUnitOfWork unitOfWork, ILogger<ConnectLockerHandler> logger, IMapper mapper, IRabbitMqBus rabbitMqBus)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task<LockerResponse> Handle(ConnectLockerCommand request, CancellationToken cancellationToken)
    {
        // Check locker by code
        var locker = await _unitOfWork.LockerRepository.FindByCode(request.Code);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        locker.Status = LockerStatus.Active;
        locker.MacAddress = request.MacAddress;
        locker.IpAddress = request.IpAddress;
        await _unitOfWork.LockerRepository.UpdateAsync(locker);
        
        var connectEvent = new LockerTimeline()
        {
            Locker = locker,
            Event = LockerEvent.Connect,
            Status = locker.Status
        };
        
        await _unitOfWork.LockerTimelineRepository.AddAsync(connectEvent);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Locker {0} connected to server", locker.Id);
        
        await _rabbitMqBus.PublishAsync(new LockerConnectedEvent()
        {
            LockerCode = locker.Code,
        }, cancellationToken);
        
        return _mapper.Map<LockerResponse>(locker);
    }
}