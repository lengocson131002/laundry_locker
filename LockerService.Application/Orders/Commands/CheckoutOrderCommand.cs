using LockerService.Application.Bills.Models;

namespace LockerService.Application.Orders.Commands;

public class CheckoutOrderCommand : IRequest<BillResponse>
{
    [JsonIgnore]
    public long Id { get; set; }
    
    public PaymentMethod Method { get; set; }
}