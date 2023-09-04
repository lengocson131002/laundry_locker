namespace LockerService.Application.EventBus.RabbitMq.Events.Accounts;

public class StaffCreatedEvent : RabbitMqBaseEvent
{
    public long AccountId { get; set; }

    public string PhoneNumber { get; set; } = default!;

    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string FullName { get; set; } = default!;
    
    public Role Role { get; set; }
}