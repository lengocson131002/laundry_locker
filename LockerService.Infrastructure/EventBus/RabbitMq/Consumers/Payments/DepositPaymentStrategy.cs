using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.EventBus.RabbitMq.Consumers.Payments;

public class DepositPaymentStrategy : IPaymentStrategy
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DepositPaymentStrategy> _logger;

    private readonly INotifier _notifier;

    public DepositPaymentStrategy(IUnitOfWork unitOfWork, ILogger<DepositPaymentStrategy> logger, INotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _notifier = notifier;
    }

    public PaymentType Type => PaymentType.Deposit;

    public async Task Handle(Payment payment)
    {
        _logger.LogInformation("Handle deposit wallet when payment completed");
        if (!Equals(payment.Status, PaymentStatus.Completed))
        {
            return;
        }
        
        // get customer
        var customer = await _unitOfWork.AccountRepository
            .Get(acc => acc.Id == payment.CustomerId)
            .Include(acc => acc.Wallet)
            .FirstOrDefaultAsync();

        if (customer == null)
        {
            return;
        }

        var wallet = customer.Wallet;
        if (wallet == null)
        {
            _logger.LogInformation($"Customer {customer.Id}'s wallet not found.Create wallet for customer");
            wallet = new Wallet(payment.Amount);
            wallet.LastDepositAt = DateTimeOffset.UtcNow;
            await _unitOfWork.WalletRepository.AddAsync(wallet);
        }
        else
        {
            wallet.Balance += payment.Amount;
            wallet.LastDepositAt = DateTimeOffset.UtcNow;
            await _unitOfWork.WalletRepository.UpdateAsync(wallet);
        }

        // Save all changes
        await _unitOfWork.SaveChangesAsync();
        
        // Push notifications
        await _notifier.NotifyAsync(new Notification(
                customer,
                NotificationType.CustomerDepositCompleted,
                EntityType.Payment,
                payment
            ));
    }
}