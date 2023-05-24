using LockerService.Application.Common.Extensions;
using LockerService.Application.Locations.Commands;
using LockerService.Application.Services.Commands;

namespace LockerService.Application.Lockers.Commands;

public class AddLockerCommandValidator : AbstractValidator<AddLockerCommand>
{
    public AddLockerCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
            .NotEmpty();
        
        RuleFor(model => model.Description)
            .NotEmpty()
            .When(model => model.Description is not null);
        
        RuleFor(model => model.Height)
            .GreaterThan(0)
            .When(model => model.Height is not null);
        
        RuleFor(model => model.Width)
            .GreaterThan(0)
            .When(model => model.Width is not null);
        
        RuleFor(model => model.Depth)
            .GreaterThan(0)
            .When(model => model.Depth is not null);
        
        RuleFor(model => model.Location)
            .NotNull()
            .SetInheritanceValidator(v => { v.Add(new AddLocationCommandValidator()); });
        
        RuleForEach(model => model.Hardwares)
            .SetInheritanceValidator(v => { v.Add(new AddHardwareCommandValidator()); });
        
        RuleForEach(model => model.Services)
            .SetInheritanceValidator(v => { v.Add(new AddServiceCommandValidator()); });
        
        RuleFor(model => model.RowCount)
            .NotNull()
            .GreaterThan(0)
            .LessThan(100);
        
        RuleFor(model => model.ColumnCount)
            .NotNull()
            .GreaterThan(0)
            .LessThan(100);
        
        RuleFor(model => model.Location)
            .NotNull();
        
        RuleFor(model => model.MacAddress)
            .NotEmpty()
            .Must(macAddress => macAddress.IsValidMacAddress())
            .WithMessage("Invalid MAC address. Right Format: XX:XX:XX:XX:XX:XX");
    }
}

public class AddLockerCommand : IRequest<LockerResponse>
{
    public string Name { get; set; } = default!;
    
    public string? Description { get; set; } = default!;
    
    public int? Height { get; set; }
    
    public int? Width { get; set; }
    
    public int? Depth { get; set; }
    
    public LocationCommand Location { get; set; } = default!;
    
    public IEnumerable<AddHardwareCommand>? Hardwares { get; set; }

    public IEnumerable<AddServiceCommand>? Services { get; set; }
    
    public int RowCount { get; set; }
    
    public int ColumnCount { get; set; }

    public string? Provider { get; set; }

    public string MacAddress { get; set; } = default!;
}