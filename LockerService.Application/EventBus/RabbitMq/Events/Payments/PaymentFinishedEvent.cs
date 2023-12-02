namespace LockerService.Application.EventBus.RabbitMq.Events.Payments;

public class PaymentFinishedEvent : RabbitMqBaseEvent
{
    public long PaymentId { get; set; }
    
}