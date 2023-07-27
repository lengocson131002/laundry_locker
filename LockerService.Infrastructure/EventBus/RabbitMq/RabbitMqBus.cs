using LockerService.Application.EventBus.RabbitMq;
using LockerService.Application.EventBus.RabbitMq.Events;

namespace LockerService.Infrastructure.EventBus.RabbitMq;

public class RabbitMqBus : IRabbitMqBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public RabbitMqBus(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : RabbitMqBaseEvent
    {
        await _publishEndpoint.Publish(message, cancellationToken);
    }
}