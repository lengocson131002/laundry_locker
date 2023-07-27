namespace LockerService.Application.Lockers.Commands;

public class AddLockerCommandValidator : AbstractValidator<AddLockerCommand>
{
    public AddLockerCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(model => model.Location)
            .NotNull()
            .SetInheritanceValidator(v => { v.Add(new AddLocationCommandValidator()); });

        RuleFor(model => model.Location)
            .NotNull();

        RuleFor(model => model.StoreId)
            .NotNull();

        RuleFor(model => model.StaffIds)
            .NotEmpty()
            .Must(UniqueStaffs)
            .WithMessage("StaffIds must contains unique ids");
    }
    
    private bool UniqueStaffs(IList<long> staffIds)
    {
        var encounteredIds = new HashSet<long>();

        foreach (var element in staffIds)
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

public class AddLockerCommand : IRequest<LockerResponse>
{
    [TrimString(true)]
    public string Name { get; set; } = default!;

    [TrimString(true)]
    public string Image { get; set; } = default!;
    
    public LocationCommand Location { get; set; } = default!;
    
    [TrimString(true)]
    public string? Description { get; set; } = default!;

    public long StoreId { get; set; }

    public IList<long> StaffIds { get; set; } = default!;
}