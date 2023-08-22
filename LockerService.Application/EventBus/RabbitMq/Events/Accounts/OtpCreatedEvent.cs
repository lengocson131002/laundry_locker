namespace LockerService.Application.EventBus.RabbitMq.Events.Accounts;

public class OtpCreatedEvent : RabbitMqBaseEvent
{
    public long AccountId { get; set; }

    public string Otp { get; set; } = default!;
}