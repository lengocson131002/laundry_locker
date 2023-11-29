namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderOvertimeProcessingEvent : RabbitMqBaseEvent
{
    public long OrderId { get; set; } = default!;

    public DateTimeOffset Time { get; set; }

    public OrderStatus PreviousStatus { get; set; }

    public Account Staff { get; set; } = default!;

    public int ReceiveBoxNumber { get; set; }

}