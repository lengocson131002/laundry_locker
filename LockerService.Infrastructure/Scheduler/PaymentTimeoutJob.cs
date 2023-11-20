using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using Quartz;

namespace LockerService.Infrastructure.Scheduler;

public class PaymentTimeoutJob : IJob
{
    public const string PaymentIdKey = "paymentId";

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<PaymentTimeoutJob> _logger;

    public PaymentTimeoutJob(IUnitOfWork unitOfWork, ILogger<PaymentTimeoutJob> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;

        var paymentId = dataMap.GetLongValue(PaymentIdKey);

        if (paymentId == 0)
        {
            return;
        }

        var payment = await _unitOfWork.PaymentRepository
            .Get(payment => payment.Id == paymentId && Equals(payment.Status, PaymentStatus.Created))
            .FirstOrDefaultAsync();
        
        if (payment == null)
        {
            return;
        }

        payment.Status = PaymentStatus.Failed;
        await _unitOfWork.PaymentRepository.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation($"Cancel expired payment {payment.Id}");

    }
}