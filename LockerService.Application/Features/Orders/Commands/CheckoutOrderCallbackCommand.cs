namespace LockerService.Application.Features.Orders.Commands;

public class CheckoutOrderCallbackCommand : IRequest
{
    [JsonIgnore]
    public long Id { get; set; }
}