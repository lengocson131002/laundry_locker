namespace LockerService.Application.Hardwares.Commands;

public class AddHardwareCommandValidator : AbstractValidator<AddHardwareCommand>
{
    public AddHardwareCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
            .NotEmpty();
        RuleFor(model => model.Code)
            .MaximumLength(200);
        RuleFor(model => model.Brand)
            .MaximumLength(200)
            .NotEmpty();
        RuleFor(model => model.Price)
            .GreaterThan(0)
            .When(model => model.Price != null);
    }
}

public class AddHardwareCommand : IRequest<HardwareResponse>
{
    [JsonIgnore]
    public long LockerId { get; set; }
 
    [TrimString(true)]
    public string Name { get; set; } = default!;
   
    [TrimString(true)]
    public string? Code { get; set; }
    
    [TrimString(true)]
    public string? Brand { get; set; }
    
    public double? Price { get; set; }
    
    [TrimString(true)]
    public string? Description { get; set; }
}