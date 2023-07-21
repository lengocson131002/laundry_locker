using LockerService.Application.Common.Extensions;
using LockerService.Application.Locations.Commands;

namespace LockerService.Application.Lockers.Commands;

public class UpdateLockerCommandValidator : AbstractValidator<UpdateLockerCommand>
{
    public UpdateLockerCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
            .NotEmpty()
            .When(model => !string.IsNullOrWhiteSpace(model.Name));


        RuleFor(model => model.Location)
            .SetInheritanceValidator(v => v.Add(new AddLocationCommandValidator()));
    }
}

public class UpdateLockerCommand : IRequest
{
    [JsonIgnore] public long LockerId { get; set; }

    public string? Name { get; set; }

    public string? Image { get; set; }
    public string? Description { get; set; }

    public LocationCommand? Location { get; set; }

    public long? StoreId { get; set; }

    public IList<long>? StaffIds { get; set; }
}