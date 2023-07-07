namespace LockerService.Application.Services.Commands;

public class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
            .When(model => model.Name is not null);
        
        RuleFor(model => model.Price)
            .GreaterThan(0)
            .When(model => model.Price is not null);
    }
}

public class UpdateServiceCommand : IRequest
{
    
    [JsonIgnore] 
    public int ServiceId { get; set; }
    
    public string? Name { get; set; }
    
    public string? Image { get; set; }

    public decimal? Price { get; set; }

    public string? Unit { get; set; }
    public string? Description { get; set; }
}