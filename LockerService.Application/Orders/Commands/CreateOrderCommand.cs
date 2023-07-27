using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Utils;

namespace LockerService.Application.Orders.Commands;

public class CreateOrderValidation : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidation()
    {
        RuleFor(req => req.LockerId)
            .NotNull();
        RuleFor(req => req.Type)
            .NotNull();
        RuleFor(req => req.SenderPhone)
            .NotNull()
            .Must(x => x.IsValidPhoneNumber())
            .WithMessage("Invalid sender phone number");

        RuleFor(req => req.ReceiverPhone)
            .Must(x => x == null || x.IsValidPhoneNumber())
            .WithMessage("Invalid receiver phone number");

        RuleFor(model => model.ServiceIds)
            .Must(serviceIds => serviceIds.Any())
            .When(order => OrderType.Laundry.Equals(order.Type))
            .WithMessage("Services is required for laundry order")
            .Must(UniqueServices)
            .When(order => OrderType.Laundry.Equals(order.Type))
            .WithMessage("ServiceIds must contains unique ids");
    }

    public bool UniqueServices(IList<long> serviceIds)
    {
        var encounteredIds = new HashSet<long>();

        foreach (var element in serviceIds)
        {
            if (!encounteredIds.Contains(element))
            {
               encounteredIds.Add(element);
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}

public class CreateOrderCommand : IRequest<OrderResponse>
{
    public long LockerId { get; set; }
    
    public OrderType Type { get; set; }
    
    [TrimString(true)]
    public string SenderPhone { get; set;  } = default!;

    [TrimString(true)]
    public string? ReceiverPhone { get; set; }

    public IList<long> ServiceIds { get; set; } = new List<long>();
}