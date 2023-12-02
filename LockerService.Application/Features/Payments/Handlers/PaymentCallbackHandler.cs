using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.EventBus.RabbitMq.Events.Orders;
using LockerService.Application.EventBus.RabbitMq.Events.Payments;
using LockerService.Application.Features.Payments.Commands;
using LockerService.Domain;

namespace LockerService.Application.Features.Payments.Handlers;

public class PaymentCallbackHandler : IRequestHandler<PaymentCallbackCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IRabbitMqBus _rabbitMqBus;

    private readonly ILogger<PaymentCallbackHandler> _logger;

    public PaymentCallbackHandler(IUnitOfWork unitOfWork, IRabbitMqBus rabbitMqBus, ILogger<PaymentCallbackHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
        _logger = logger;
    }

    public async Task Handle(PaymentCallbackCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.PaymentRepository
            .Get(payment => payment.ReferenceId == request.PaymentReferenceId 
                            && Equals(payment.Status, PaymentStatus.Created))
            .FirstOrDefaultAsync(cancellationToken);

        if (payment == null)
        {
            throw new ApiException(ResponseCode.PaymentErrorNotFound);
        }

        payment.Status = request.IsSuccess 
            ? PaymentStatus.Completed 
            : PaymentStatus.Failed;
        payment.ReferenceTransactionId = request.ReferenceTransactionId;
        await _unitOfWork.PaymentRepository.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        // Push rabbitMQ Event
        await _rabbitMqBus.PublishAsync(new PaymentFinishedEvent()
        {
            PaymentId = payment.Id
        }, cancellationToken);
        
        _logger.LogInformation("Payment callback from {0}. IsSuccess: {1}, Amount: {2}", payment.Method, request.IsSuccess, request.Amount);
    }

}