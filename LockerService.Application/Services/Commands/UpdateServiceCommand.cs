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

        RuleFor(model => model.Image)
            .Must(image => image == null || image.IsValidUrl())
            .WithMessage("Invalid image url");
    }
}

public class UpdateServiceCommand : IRequest
{
    [JsonIgnore]
    public long StoreId { get; set; }
    
    [JsonIgnore] 
    public long ServiceId { get; set; }
    
    [TrimString(true)]
    public string? Name { get; set; }
    
    [TrimString(true)]
    public string? Image { get; set; }

    public decimal? Price { get; set; }
    
    [TrimString]
    public string? Unit { get; set; }
    
    [TrimString]
    public string? Description { get; set; }
}