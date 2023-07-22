using System.Text.Json;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using LockerService.Domain.Events;
using MassTransit;

namespace LockerService.Application.EventBus.RabbitMq.Consumers.Lockers;

public class LockerConnectedConsumer : IConsumer<LockerConnectedEvent>
{
    private readonly ILogger<LockerConnectedConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public LockerConnectedConsumer(ILogger<LockerConnectedConsumer> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<LockerConnectedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received locker connected message: {0}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(message.LockerId);
        if (locker == null)
        {
            return;
        }

        var @event = new LockerTimeline()
        {
            LockerId = locker.Id,
            Event = LockerEvent.Connect,
            Data = JsonSerializer.Serialize(locker),
            Status = locker.Status
        };
        
        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();
    }
}