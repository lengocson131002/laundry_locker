using LockerService.Domain;

namespace LockerService.Application.Common.Services.Payments;

public interface IPaymentService
{
    public Task<Payment> Checkout(Order order, PaymentMethod method, CancellationToken cancellationToken);

    public Task SetPaymentTimeOut(Payment payment, DateTimeOffset time);

    public Task<Payment> CreatePayment(MomoPayment momoPayment);
    
    public Task<Payment> CreatePayment(VnPayPayment vnPayPayment);
}