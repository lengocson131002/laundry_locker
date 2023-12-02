using LockerService.Domain;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Payments;

public interface IPaymentStrategyContext
{
    public Task ExecuteStrategy(Payment payment);
}