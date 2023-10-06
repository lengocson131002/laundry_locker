using LockerService.Application.Features.Orders.Models;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Orders.Commands;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(model => model.Image)
            .Must(image => image == null || image.IsValidUrl())
            .WithMessage("Invalid image url");
    }
}

public class UpdateOrderStatusCommand : IRequest<OrderResponse>
{
    [JsonIgnore]
    [BindNever]
    public long OrderId { get; set; }
    
    public OrderStatus OrderStatus { get; set; }
    
    [TrimString]
    public string? Image { get; set; }
    
    [TrimString]
    public string? Description { get; set; }
}