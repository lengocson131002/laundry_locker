using LockerService.Application.EventBus.RabbitMq.Events;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;

namespace LockerService.Application.Common.Mappings;

public class RabbitMqEventMappings : Profile
{
    public RabbitMqEventMappings()
    {
        CreateMap<Order, OrderCreatedEvent>();
        CreateMap<Order, OrderReturnedEvent>();
    }
}