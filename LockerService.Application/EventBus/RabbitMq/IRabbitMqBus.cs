using LockerService.Application.EventBus.RabbitMq.Events;

namespace LockerService.Application.EventBus.RabbitMq;

public interface IRabbitMqBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : RabbitMqBaseEvent;
}