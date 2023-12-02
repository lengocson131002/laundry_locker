using LockerService.Domain;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Payments;

public class PaymentStrategyContext : IPaymentStrategyContext
{
    private readonly IEnumerable<IPaymentStrategy> _strategies;

    public PaymentStrategyContext(IEnumerable<IPaymentStrategy> strategies)
    {
        _strategies = strategies;
    }
    
    public async Task ExecuteStrategy(Payment payment)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Type == payment.Type);
        if (strategy == null)
        {
            throw new Exception($"No payment strategy supported for payment type: {payment.Type}");
        }

        await strategy.Handle(payment);
    }
}