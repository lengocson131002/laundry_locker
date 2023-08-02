namespace LockerService.Application.Stores.Commands;

public class AddStoreCommandValidator : AbstractValidator<AddStoreCommand>
{
    public AddStoreCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(model => model.ContactPhone)
            .Must(contactPhone => contactPhone.IsValidPhoneNumber())
            .WithMessage("Invalid Contact Phone")
            .NotEmpty();

        RuleFor(model => model.Location)
            .NotNull()
            .SetInheritanceValidator(v => { v.Add(new AddLocationCommandValidator()); });

        RuleFor(model => model.ContactPhone)
            .MaximumLength(20)
            .When(model => model is not null);

        RuleFor(model => model.Image)
            .MaximumLength(1000)
            .When(model => model is not null);
    }
}

public class AddStoreCommand : IRequest<StoreResponse>
{
    [TrimString(true)]
    public string Name { get; set; } = default!;

    [TrimString(true)]
    public string ContactPhone { get; set; } = default!;

    public LocationCommand Location { get; set; } = default!;

    [TrimString(true)]
    public string? Image { get; set; }
}