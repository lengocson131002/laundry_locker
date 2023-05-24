namespace LockerService.Application.Services.Commands;

public class AddServiceCommandValidator : AbstractValidator<AddServiceCommand>
{
    public AddServiceCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(model => model.Image)
            .NotEmpty();
        
        RuleFor(model => model.Fee)
            .GreaterThan(0)
            .NotNull()
            .When(model => !Equals(model.FeeType, FeeType.ByInputPrice));
        
        RuleFor(model => model.FeeType)
            .IsInEnum()
            .NotNull();
    }
}

public class AddServiceCommand : IRequest<ServiceResponse>
{
    [JsonIgnore] 
    public int LockerId { get; set; }

    public string Name { get; set; } = default!;

    public string Image { get; set; } = default!;

    public double? Fee { get; set; }
    
    public FeeType FeeType { get; set; }
    
    public string? Description { get; set; }
    
    public string? Unit { get; set; }
}