using LockerService.Application.EventBus.RabbitMq.Events;

namespace LockerService.Application.Common.Mappings;

public class RabbitMqEventMappings : Profile
{
    public RabbitMqEventMappings()
    {
        CreateMap<Order, OrderCreatedEvent>();
        CreateMap<Order, OrderReturnedEvent>();
    }
}