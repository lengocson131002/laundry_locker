using LockerService.Application.Features.Payments.Models;

namespace LockerService.Application.Features.Orders.Commands;

public class CheckoutOrderCommand : IRequest<PaymentResponse>
{
    [JsonIgnore]
    public long Id { get; set; }
    
    public PaymentMethod Method { get; set; }
    
}