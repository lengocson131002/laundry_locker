namespace LockerService.Application.Stores.Commands;

public class AddStoreCommandValidator : AbstractValidator<AddStoreCommand>
{
    public AddStoreCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(model => model.ContactPhone)
            .Must(contactPhone => contactPhone == null || contactPhone.IsValidPhoneNumber())
            .WithMessage("Invalid Contact Phone")
            .NotEmpty();

        RuleFor(model => model.Location)
            .NotNull()
            .SetInheritanceValidator(v => { v.Add(new AddLocationCommandValidator()); });

        RuleFor(model => model.ContactPhone)
            .MaximumLength(20)
            .Must(phone => phone == null || phone.IsValidPhoneNumber())
            .WithMessage("Invalid contact phone number");

        RuleFor(model => model.Image)
            .MaximumLength(1000)
            .Must(image => image.IsValidUrl())
            .When(model => model.Image is not null)
            .WithMessage("Invalid image url");
    }
}

public class AddStoreCommand : IRequest<StoreResponse>
{
    [TrimString(true)]
    public string Name { get; set; } = default!;

    [NormalizePhone(true)]
    public string? ContactPhone { get; set; } = default!;

    public LocationCommand Location { get; set; } = default!;

    [TrimString(true)]
    public string? Image { get; set; }
    
    [TrimString]
    public string? Description { get; set; }
}