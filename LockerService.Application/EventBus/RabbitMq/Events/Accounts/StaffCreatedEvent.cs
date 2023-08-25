namespace LockerService.Application.EventBus.RabbitMq.Events.Accounts;

public class StaffCreatedEvent : RabbitMqBaseEvent
{
    public long AccountId { get; set; }
    
    public string? Username { get; set; }
    
    public string? Password { get; set; }
    
    public string? FullName { get; set; }
    
    public Role? Role { get; set; }
}