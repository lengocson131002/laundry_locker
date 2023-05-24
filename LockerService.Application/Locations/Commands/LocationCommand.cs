namespace LockerService.Application.Locations.Commands;

public class AddLocationCommandValidator : AbstractValidator<LocationCommand>
{
    public AddLocationCommandValidator()
    {
        RuleFor(model => model.Address)
            .NotEmpty();
        
        RuleFor(model => model.WardCode)
            .NotEmpty();
        
        RuleFor(model => model.DistrictCode)
            .NotEmpty();
        
        RuleFor(model => model.ProvinceCode)
            .NotEmpty();
        
        RuleFor(model => model.Longitude)
            .GreaterThan(0)
            .When(model => model.Longitude != null);
        
        RuleFor(model => model.Latitude)
            .GreaterThan(0)
            .When(model => model.Latitude != null);
    }
}

public class LocationCommand : IRequest
{
    public string Address { get; set; } = default!;
    
    public string WardCode { get; set; } = default!;
    
    public string DistrictCode { get; set; } = default!;
    
    public string ProvinceCode { get; set; } = default!;

    public double? Longitude { get; set; } = default!;

    public double? Latitude { get; set; } = default;
}