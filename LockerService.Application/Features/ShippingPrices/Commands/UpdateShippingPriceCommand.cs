using LockerService.Application.Features.ShippingPrices.Models;

namespace LockerService.Application.Features.ShippingPrices.Commands;

public class UpdateShippingPriceCommandValidator : AbstractValidator<UpdateShippingPriceCommand>
{
    public UpdateShippingPriceCommandValidator()
    {
 
    }
}

public class UpdateShippingPriceCommand : IRequest<ShippingPriceResponse>
{
    [JsonIgnore]
    public long ShippingPriceId { get; set; }
    
    public double? FromDistance { get; set; }
    
    public decimal? Price { get; set; }
    
}