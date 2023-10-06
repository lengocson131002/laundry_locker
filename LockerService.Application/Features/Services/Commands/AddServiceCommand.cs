using LockerService.Application.Features.Services.Models;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Services.Commands;

public class AddServiceCommandValidator : AbstractValidator<AddServiceCommand>
{
    public AddServiceCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
            .NotEmpty();

        RuleFor(model => model.Image)
            .NotEmpty()
            .Must(image => image.IsValidUrl())
            .WithMessage("Invalid image url");

        RuleFor(model => model.Price)
            .GreaterThan(0);
    }
}

public class AddServiceCommand : IRequest<ServiceResponse>
{
    [JsonIgnore]
    public long StoreId { get; set; }
    
    [TrimString(true)]
    public string Name { get; set; } = default!;

    [TrimString(true)]
    public string Image { get; set; } = default!;

    public decimal Price { get; set; } = default!;
    
    [TrimString(true)]
    public string? Unit { get; set; }

    [TrimString(true)]
    public string? Description { get; set; }
    
}