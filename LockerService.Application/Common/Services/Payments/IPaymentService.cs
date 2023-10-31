using LockerService.Domain;

namespace LockerService.Application.Common.Services.Payments;

public interface IPaymentService
{
    public Task<Payment> Checkout(Order order, PaymentMethod method, CancellationToken cancellationToken);
}