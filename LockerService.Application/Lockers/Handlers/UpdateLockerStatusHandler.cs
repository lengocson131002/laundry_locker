using LockerService.Application.EventBus.RabbitMq;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;

namespace LockerService.Application.Lockers.Handlers;

public class UpdateLockerStatusHandler :
    IRequestHandler<UpdateLockerStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRabbitMqBus _rabbitMqBus;

    public UpdateLockerStatusHandler(IUnitOfWork unitOfWork, IRabbitMqBus rabbitMqBus)
    {
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task Handle(UpdateLockerStatusCommand request, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(request.LockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        if (!locker.CanUpdateStatus || Equals(request.Status, locker.Status))
        {
            throw new ApiException(ResponseCode.LockerErrorInvalidStatus);
        }

        var currentStatus = locker.Status;
        if (request.Status.Equals(currentStatus)) return;

        locker.Status = request.Status;
        await _unitOfWork.LockerRepository.UpdateAsync(locker);
        await _unitOfWork.SaveChangesAsync();

        await _rabbitMqBus.PublishAsync(new LockerUpdatedStatusEvent()
        {
            LockerId = locker.Id,
            Status = locker.Status,
            PreviousStatus = currentStatus
        }, cancellationToken);
    }
}