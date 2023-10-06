using LockerService.Application.Features.Locations.Commands;
using LockerService.Application.Features.Lockers.Models;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Lockers.Commands;

public class AddLockerCommandValidator : AbstractValidator<AddLockerCommand>
{
    public AddLockerCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(model => model.Location)
            .NotNull()
            .SetInheritanceValidator(v => { v.Add(new AddLocationCommandValidator()); });

        RuleFor(model => model.Location)
            .NotNull();

        RuleFor(model => model.StoreId)
            .NotNull();

        RuleFor(model => model.Image)
            .Must(image => image == null || image.Trim().IsValidUrl())
            .WithMessage("Invalid image url");

        RuleFor(model => model.OrderTypes)
            .Must(orderTypes => orderTypes != null && orderTypes.Count > 0 && orderTypes.Distinct().Count() == orderTypes.Count())
            .WithMessage("OrderType must not empty and contains unique values");
    }
    
}

public class AddLockerCommand : IRequest<LockerResponse>
{
    [TrimString(true)]
    public string Name { get; set; } = default!;

    [TrimString(true)]
    public string? Image { get; set; } = default!;
    
    public LocationCommand Location { get; set; } = default!;
    
    [TrimString(true)]
    public string? Description { get; set; } = default!;

    public long StoreId { get; set; }

    public IList<OrderType> OrderTypes { get; set; } = new List<OrderType>();

}