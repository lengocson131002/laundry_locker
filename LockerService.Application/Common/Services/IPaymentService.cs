namespace LockerService.Application.Common.Services;

public interface IPaymentService
{
    Task Pay(Order order, PaymentMethod method);
}