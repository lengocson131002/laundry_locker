using System.Text.Json;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Domain.Events;
using MassTransit;

namespace LockerService.Application.EventBus.RabbitMq.Consumers.Lockers;

public class LockerUpdatedInfoConsumer : IConsumer<LockerUpdatedInfoEvent>
{
    private readonly ILogger<LockerUpdatedInfoConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LockerUpdatedInfoConsumer(ILogger<LockerUpdatedInfoConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<LockerUpdatedInfoEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received locker updated message: {0}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(message.Id);
        if (locker == null)
        {
            return;
        }

        var @event = new LockerTimeline()
        {
            LockerId = locker.Id,
            Event = LockerEvent.UpdateInformation,
            Status = locker.Status,
            Data = message.Data
        };

        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();
    }
}