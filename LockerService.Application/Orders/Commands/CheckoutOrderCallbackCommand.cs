namespace LockerService.Application.Orders.Commands;

public class CheckoutOrderCallbackCommand : IRequest
{
    [JsonIgnore]
    public long Id { get; set; }
}