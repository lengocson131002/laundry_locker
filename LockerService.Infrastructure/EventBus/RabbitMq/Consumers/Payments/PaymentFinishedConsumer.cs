using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.EventBus.RabbitMq;
using LockerService.Application.EventBus.RabbitMq.Events.Payments;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Payments;

public class PaymentFinishedConsumer : IConsumer<PaymentFinishedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<PaymentFinishedConsumer> _logger;

    private readonly IRabbitMqBus _rabbitMqBus;

    private readonly IPaymentStrategyContext _paymentStrategyContext;

    public PaymentFinishedConsumer(
        IUnitOfWork unitOfWork, 
        ILogger<PaymentFinishedConsumer> logger, 
        IRabbitMqBus rabbitMqBus, 
        IPaymentStrategyContext paymentStrategyContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _rabbitMqBus = rabbitMqBus;
        _paymentStrategyContext = paymentStrategyContext;
    }

    public async Task Consume(ConsumeContext<PaymentFinishedEvent> context)
    {
        // Retrieve payment information
        var message = context.Message;
        var paymentId = message.PaymentId;
        var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
        
        // Cancel if payment is created
        if (payment == null || !payment.Finished)
        {
            return;
        }

        // Handle payment
        _logger.LogInformation($"Payment {payment.Id} finished at {DateTimeOffset.Now}");
        await _paymentStrategyContext.ExecuteStrategy(payment);
    }
    
}