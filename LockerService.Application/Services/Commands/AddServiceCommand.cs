using LockerService.Application.Common.Utils;

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

        RuleFor(model => model.Price)
            .GreaterThan(0);
    }
}

public class AddServiceCommand : IRequest<ServiceResponse>
{
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