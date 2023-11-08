using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Services.Commands;

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
    
    public ServiceStatus? Status { get; set; }
}