namespace LockerService.Application.EventBus.RabbitMq.Events.Orders;

public class OrderUpdatedStatusEvent : RabbitMqBaseEvent
{
    public long OrderId { get; set; }
    
    public OrderStatus? PreviousStatus { get; set; }

    public OrderStatus Status { get; set; }
    
    public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;
    
    public long? StaffId { get; set; }
    
    public string? Image { get; set; }
    
    public string? Description { get; set; }
}