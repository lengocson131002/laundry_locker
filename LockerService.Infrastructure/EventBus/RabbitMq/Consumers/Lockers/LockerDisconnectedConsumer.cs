using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerDisconnectedConsumer : IConsumer<LockerDisconnectedEvent>
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LockerDisconnectedConsumer(ILogger<LockerDisconnectedConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<LockerDisconnectedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Receive locker disconnected message: {0}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository
            .Get(locker => locker.Code == message.LockerCode)
            .FirstOrDefaultAsync();
        
        if (locker == null)
        {
            return;
        }

        locker.Status = LockerStatus.Disconnected;
        await _unitOfWork.LockerRepository.UpdateAsync(locker);
        
        var @event = new LockerTimeline()
        {
            LockerId = locker.Id,
            Event = LockerEvent.Disconnect,
            Status = locker.Status,
            Data = JsonSerializer.Serialize(locker),
        };

        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();
    }
}