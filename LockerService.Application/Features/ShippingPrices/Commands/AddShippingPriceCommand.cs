using LockerService.Application.Features.ShippingPrices.Models;

namespace LockerService.Application.Features.ShippingPrices.Commands;

public class AddShippingPriceCommandValidator : AbstractValidator<AddShippingPriceCommand>
{
    public AddShippingPriceCommandValidator()
    {
    
    }
}

public class AddShippingPriceCommand : IRequest<ShippingPriceResponse>
{
    public double FromDistance { get; set; }
    
    public decimal Price { get; set; }
}