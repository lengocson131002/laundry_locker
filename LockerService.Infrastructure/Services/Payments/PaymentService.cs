using LockerService.Application.Common.Services.Payments;
using LockerService.Domain;
using LockerService.Domain.Enums;

namespace LockerService.Infrastructure.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IMomoPaymentService _momoPaymentService;
    
    private readonly IVnPayPaymentService _vnPayPaymentService;

    public PaymentService(IMomoPaymentService momoPaymentService, IVnPayPaymentService vnPayPaymentService)
    {
        _momoPaymentService = momoPaymentService;
        _vnPayPaymentService = vnPayPaymentService;
    }

    public async Task<Payment> Checkout(Order order, PaymentMethod method, CancellationToken cancellationToken)
    {
        return method switch
        {
            PaymentMethod.Cash => await HandleCashCheckout(order, cancellationToken),
            PaymentMethod.Momo => await HandleMomoCheckout(order, cancellationToken),
            PaymentMethod.VnPay => await HandleVnPayCheckout(order, cancellationToken),
            _ => throw new Exception("Invalid payment method")
        };
    }
    
    private async Task<Payment> HandleVnPayCheckout(Order order, CancellationToken cancellationToken)
    {
        var amount = (long)(order.CalculateTotalPrice() - order.ReservationFee);
        var payment = await _vnPayPaymentService.CreatePayment(new VnPayPayment()
        {
            Amount = amount,
            OrderReferenceId = order.ReferenceId ?? throw new Exception("Order referenceId is required"),
            PaymentReferenceId = Guid.NewGuid().ToString(),
            Info = Payment.PaymentContent(order.Type),
            Time = DateTimeOffset.UtcNow
        });

        payment.OrderId = order.Id;
        payment.CustomerId = order.ReceiverId ?? order.SenderId;
        payment.StoreId = order.Locker.StoreId;
        
        return payment;
    }

    private async Task<Payment> HandleMomoCheckout(Order order, CancellationToken cancellationToken)
    {
        var amount = (long) (order.CalculateTotalPrice() - order.ReservationFee);
        
        var payment = await _momoPaymentService.CreatePayment(new MomoPayment()
        {
            Amount = amount,
            OrderReferenceId = order.ReferenceId ?? throw new Exception("Order referenceId is required"),
            PaymentReferenceId = Guid.NewGuid().ToString(),
            Info =  Payment.PaymentContent(order.Type)
        });

        payment.OrderId = order.Id;
        payment.CustomerId = order.ReceiverId ?? order.SenderId;
        payment.StoreId = order.Locker.StoreId;
        
        return payment;
    }

    private async Task<Payment> HandleCashCheckout(Order order, CancellationToken cancellationToken)
    {
        var payment = new Payment(order, PaymentMethod.Cash, PaymentStatus.Completed);
        return await Task.FromResult(payment);
    }
}