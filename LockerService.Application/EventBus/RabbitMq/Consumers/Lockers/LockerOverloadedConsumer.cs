using System.Text.Json;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Domain.Events;
using MassTransit;

namespace LockerService.Application.EventBus.RabbitMq.Consumers.Lockers;

public class LockerOverloadedConsumer : IConsumer<LockerOverloadedEvent>
{
    private readonly ILogger<LockerOverloadedConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LockerOverloadedConsumer(ILogger<LockerOverloadedConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<LockerOverloadedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received locker overloaded event: {}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(message.LockerId);
        if (locker == null)
        {
            return;
        }

        var @event = new LockerTimeline()
        {
            LockerId = locker.Id,
            Event = LockerEvent.Overload,
            Status = locker.Status,
            Data = JsonSerializer.Serialize(locker),
            Error = message.Error,
            ErrorCode = message.ErrorCode
        };

        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();
    }
}