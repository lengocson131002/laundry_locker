using LockerService.Domain.Events;

namespace LockerService.Application.Lockers.Handlers;

public class UpdateLockerStatusHandler :
    IRequestHandler<UpdateLockerStatusCommand>
{
    private readonly IMqttBus _mqttBus;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLockerStatusHandler(IUnitOfWork unitOfWork, IMqttBus mqttBus)
    {
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
    }


    public async Task Handle(UpdateLockerStatusCommand request, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(request.LockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        var currentStatus = locker.Status;
        if (request.Status.Equals(currentStatus)) return;

        await ValidateStatus(locker, request.Status);

        locker.Status = request.Status;
        await _unitOfWork.LockerRepository.UpdateAsync(locker);

        var lockerEvent = new LockerTimeline()
        {
            Locker = locker,
            Event = LockerEvent.UpdateStatus,
            Status = request.Status,
            PreviousStatus = currentStatus,
            Time = DateTimeOffset.UtcNow
        };
        
        await _unitOfWork.LockerTimelineRepository.AddAsync(lockerEvent);
        
        await _unitOfWork.SaveChangesAsync();

        // Push MQTT event
        await _mqttBus.PublishAsync(new MqttUpdateLockerStatusEvent(locker.Id, request.Status));
    }

    private async Task ValidateStatus(Locker locker, LockerStatus status)
    {
        if (!LockerStatus.Initialized.Equals(locker.Status) && LockerStatus.Initialized.Equals(status))
            throw new ApiException(ResponseCode.LockerErrorInvalidStatus);

        var services = await _unitOfWork.ServiceRepository.GetAsync(service => service.LockerId == locker.Id);
        if (!services.Any() && LockerStatus.Active.Equals(status))
        {
            throw new ApiException(ResponseCode.LockerErrorServiceRequired);
        }
    }
}