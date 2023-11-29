using LockerService.Application.Features.Locations.Commands;
using LockerService.Application.Features.Orders.Models;
using LockerService.Shared.Extensions;
using LockerService.Shared.Utils;

namespace LockerService.Application.Features.Orders.Commands;

public class InitializeOrderCommandValidation : AbstractValidator<InitializeOrderCommand>
{
    public InitializeOrderCommandValidation()
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

        RuleFor(model => model.Details)
            .NotEmpty()
            .Must(details => details.ContainsUniqueElements(d => new {d.ServiceId}))
            .WithMessage("Details should not empty and contains unique service");

        RuleFor(model => model.DeliveryAddress)
            .SetInheritanceValidator(v => v.Add(new AddLocationCommandValidator()))
            .When(model => model.DeliveryAddress != null);
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

public class InitializeOrderCommand : IRequest<OrderDetailResponse>
{
    public long LockerId { get; set; }
    
    public OrderType Type { get; set; }
    
    [NormalizePhone(true)]
    public string SenderPhone { get; set;  } = default!;

    [NormalizePhone(true)]
    public string? ReceiverPhone { get; set; }

    public IList<OrderDetailItemCommand> Details { get; set; } = new List<OrderDetailItemCommand>();
    
    public LocationCommand? DeliveryAddress { get; set; }
    
    public DateTimeOffset? IntendedReceiveAt { get; set; }

    public bool IsReserving { get; set; } = false;
    
    public string? CustomerNote { get; set; }
}