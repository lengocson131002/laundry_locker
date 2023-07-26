using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Lockers;

public class LockerUpdatedStatusConsumer : IConsumer<LockerUpdatedStatusEvent>
{
    private readonly ILogger<LockerUpdatedStatusConsumer> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMqttBus _mqttBus;

    public LockerUpdatedStatusConsumer(ILogger<LockerUpdatedStatusConsumer> logger, IUnitOfWork unitOfWork, IMqttBus mqttBus)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
    }

    public async Task Consume(ConsumeContext<LockerUpdatedStatusEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received locker updated status message: {0}", JsonSerializer.Serialize(message));

        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(message.LockerId);
        if (locker == null)
        {
            return;
        }
        
        var @event = new LockerTimeline()
        {
            LockerId = locker.Id,
            Event = LockerEvent.UpdateStatus,
            Status = message.Status,
            PreviousStatus = message.PreviousStatus,
            Data = JsonSerializer.Serialize(locker),
        };

        await _unitOfWork.LockerTimelineRepository.AddAsync(@event);
        await _unitOfWork.SaveChangesAsync();
        
        // Push MQTT event
        await _mqttBus.PublishAsync(new MqttUpdateLockerStatusEvent(locker.Id, locker.Status));
    }
}