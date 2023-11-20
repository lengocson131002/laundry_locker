using LockerService.Application.Features.ShippingPrices.Models;

namespace LockerService.Application.Features.ShippingPrices.Commands;

public class AddShippingPriceCommandValidator : AbstractValidator<AddShippingPriceCommand>
{
    public AddShippingPriceCommandValidator()
    {
        RuleFor(model => model.FromDistance)
            .GreaterThanOrEqualTo(0);

        RuleFor(model => model.Price)
            .GreaterThanOrEqualTo(0);
    }
}

public class AddShippingPriceCommand : IRequest<ShippingPriceResponse>
{
    public double FromDistance { get; set; }
    
    public decimal Price { get; set; }
}