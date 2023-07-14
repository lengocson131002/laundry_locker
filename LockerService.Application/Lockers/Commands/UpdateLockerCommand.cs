using LockerService.Application.Common.Extensions;
using LockerService.Application.Locations.Commands;

namespace LockerService.Application.Lockers.Commands;

public class UpdateLockerCommandValidator : AbstractValidator<UpdateLockerCommand>
{
    public UpdateLockerCommandValidator()
    {
        RuleFor(model => model.Name)
            .MaximumLength(200)
            .NotEmpty()
            .When(model => !string.IsNullOrWhiteSpace(model.Name));
        
        RuleFor(model => model.Height)
            .GreaterThan(0)
            .When(model => model.Height != null);
        
        RuleFor(model => model.Width)
            .GreaterThan(0)
            .When(model => model.Width != null);
       
        RuleFor(model => model.Depth)
            .GreaterThan(0)
            .When(model => model.Depth != null);

        RuleFor(model => model.RowCount)
            .GreaterThan(0)
            .When(model => model.RowCount != null);
        
        RuleFor(model => model.ColumnCount)
            .GreaterThan(0)
            .When(model => model.ColumnCount != null);
        
        RuleFor(model => model.Location)
            .SetInheritanceValidator(v => v.Add(new AddLocationCommandValidator()));
        
        RuleFor(model => model.MacAddress)
            .Must(mac => mac == null || mac.IsValidMacAddress())
            .WithMessage("Invalid MAC address. Right Format: XX:XX:XX:XX:XX:XX");
    }
}

public class UpdateLockerCommand : IRequest
{
    [JsonIgnore] 
    public long LockerId { get; set; }
    
    public string? Name { get; set; }
    
    public int? Height { get; set; }
    
    public int? Width { get; set; }
    
    public int? Depth { get; set; }
    
    public string? Description { get; set; }
    
    public int? RowCount { get; set; }
    
    public int? ColumnCount { get; set; }

    public string? Provider { get; set; }
    
    public LocationCommand? Location { get; set; }
    
    public string? MacAddress { get; set; }
    
    public long? StoreId { get; set; }
}