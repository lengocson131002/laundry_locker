namespace LockerService.Application.Locations.Commands;

public class AddLocationCommandValidator : AbstractValidator<LocationCommand>
{
    public AddLocationCommandValidator()
    {
        RuleFor(model => model.Address)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(model => model.WardCode)
            .NotEmpty();
        
        RuleFor(model => model.DistrictCode)
            .NotEmpty();
        
        RuleFor(model => model.ProvinceCode)
            .NotEmpty();
    }
}

public class LocationCommand : IRequest
{
    [TrimString(true)]
    public string Address { get; set; } = default!;

    [TrimString(true)]
    public string WardCode { get; set; } = default!;
    
    [TrimString(true)]
    public string DistrictCode { get; set; } = default!;
    
    [TrimString(true)]
    public string ProvinceCode { get; set; } = default!;

    public double? Longitude { get; set; } = default!;

    public double? Latitude { get; set; } = default;
}