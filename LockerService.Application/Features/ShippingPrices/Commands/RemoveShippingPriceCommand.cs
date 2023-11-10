using LockerService.Application.Features.ShippingPrices.Models;

namespace LockerService.Application.Features.ShippingPrices.Commands;

public record RemoveShippingPriceCommand(long ShippingPriceId) : IRequest<ShippingPriceResponse>;
