using LockerService.Application.Features.Payments.Models;

namespace LockerService.Application.Features.Orders.Commands;

public class CheckoutOrderCommand : IRequest<PaymentResponse>
{
    [JsonIgnore]
    public long OrderId { get; set; }
    
    public PaymentMethod Method { get; set; }
    
}