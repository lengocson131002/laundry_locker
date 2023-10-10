using LockerService.Domain;

namespace LockerService.Application.Common.Services.Payments;

public interface IVnPayPaymentService
{
    public Task<Payment> CreatePayment(VnPayPayment payment);
    
}