namespace LockerService.Application.Stores.Commands;

public class AddStoreCommandValidator : AbstractValidator<AddStoreCommand>
{
    public AddStoreCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
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
    public string Name { get; set; } = default!;
    
    public string? ContactPhone { get; set; }
    
    public LocationCommand Location { get; set; } = default!;
    
    public string? Image { get; set; }
}