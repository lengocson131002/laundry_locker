using LockerService.Shared.Utils;

namespace LockerService.Application.Features.Orders.Commands;

public class OrderDetailCommandItemValidator : AbstractValidator<OrderDetailCommandItem>
{
    public OrderDetailCommandItemValidator()
    {
        RuleFor(model => model.ServiceId)
            .NotNull();
        
        RuleFor(model => model.Quantity)
            .NotNull()
            .GreaterThan(0);
    }
}

public class AddOrderDetailCommandValidator : AbstractValidator<AddOrderDetailCommand>
{
    public AddOrderDetailCommandValidator()
    {
        RuleFor(model => model.Details)
            .NotEmpty()
            .Must(details => details.ContainsUniqueElements(d => new {d.ServiceId}))
            .WithMessage("Details should not empty and contains unique service");

        RuleForEach(model => model.Details).SetValidator(new OrderDetailCommandItemValidator());

    }
}


public class AddOrderDetailCommand : IRequest<StatusResponse>
{
    [JsonIgnore]
    public long OrderId { get; set; }

    public IList<OrderDetailCommandItem> Details { get; set; } = new List<OrderDetailCommandItem>();
}

public class OrderDetailCommandItem
{
    public long ServiceId { get; set; }
    
    public float Quantity { get; set; }
}