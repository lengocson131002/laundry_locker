using System.Text.Json;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Domain.Events;
using MassTransit;

namespace LockerService.Application.EventBus.RabbitMq.Consumers.Lockers;

public class LockerDisconnectedConsumer : IConsumer<LockerDisconnectedEvent>
{
    private readonly ILogger<LockerDisconnectedConsumer> _logger;
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

        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(message.LockerId);
        if (locker == null)
        {
            return;
        }

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