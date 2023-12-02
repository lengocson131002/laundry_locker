using LockerService.Domain;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Payments;

public interface IPaymentStrategy
{
    public PaymentType Type { get; }
    
    Task Handle(Payment payment);
    
}