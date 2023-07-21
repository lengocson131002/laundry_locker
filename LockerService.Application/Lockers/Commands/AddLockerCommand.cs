using System.Collections;
using LockerService.Application.Common.Extensions;
using LockerService.Application.Locations.Commands;
using LockerService.Application.Services.Commands;

namespace LockerService.Application.Lockers.Commands;

public class AddLockerCommandValidator : AbstractValidator<AddLockerCommand>
{
    public AddLockerCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(model => model.Description)
            .NotEmpty()
            .When(model => model.Description is not null);

        RuleFor(model => model.Image)
            .NotEmpty()
            .When(model => model.Image is not null);

        RuleFor(model => model.Location)
            .NotNull()
            .SetInheritanceValidator(v => { v.Add(new AddLocationCommandValidator()); });

        RuleForEach(model => model.Hardwares)
            .SetInheritanceValidator(v => { v.Add(new AddHardwareCommandValidator()); });

        RuleFor(model => model.Location)
            .NotNull();

        RuleFor(model => model.StoreId)
            .NotNull();
    }
}

public class AddLockerCommand : IRequest<LockerResponse>
{
    public string Name { get; set; } = default!;

    public string? Image { get; set; }

    public string? Description { get; set; }

    public LocationCommand Location { get; set; } = default!;

    public IEnumerable<AddHardwareCommand>? Hardwares { get; set; }

    public long StoreId { get; set; } = default!;

    public IList<long> StaffIds { get; set; } = new List<long>();
}