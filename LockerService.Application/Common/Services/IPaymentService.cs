using LockerService.Domain;

namespace LockerService.Application.Common.Services;

public interface IPaymentService
{
    Task<Payment> Pay(Order order, PaymentMethod method);
    
    Task<Payment> InitPayment(Order order, PaymentMethod method);

}