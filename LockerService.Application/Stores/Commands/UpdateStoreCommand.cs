namespace LockerService.Application.Stores.Commands;

public class UpdateStoreCommandValidator : AbstractValidator<UpdateStoreCommand>
{
    public UpdateStoreCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
            .When(model => model.Name is not null);

        RuleFor(model => model.Location)
            .SetInheritanceValidator(v => { v.Add(new AddLocationCommandValidator()); })
            .When(model => model.Location is not null);

        RuleFor(model => model.ContactPhone)
            .Must(contactPhone => contactPhone.IsValidPhoneNumber())
            .WithMessage("Invalid Contact Phone")
            .When(model => model is not null);

        RuleFor(model => model.Image)
            .MaximumLength(1000)
            .When(model => model is not null);
    }
}

public class UpdateStoreCommand : IRequest<StoreResponse>
{
    [JsonIgnore] public long StoreId { get; set; }

    public string? Name { get; set; }
    public string? ContactPhone { get; set; }

    public LocationCommand? Location { get; set; }

    public string? Image { get; set; }
}