using LockerService.Application.Payments.Models;

namespace LockerService.Application.Orders.Commands;

public class CheckoutOrderCommand : IRequest<PaymentResponse>
{
    [JsonIgnore]
    public long Id { get; set; }
    
    public PaymentMethod Method { get; set; }
    
}