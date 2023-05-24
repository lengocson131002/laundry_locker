namespace LockerService.Application.Hardwares.Commands;

public class UpdateHardwareCommandValidator : AbstractValidator<UpdateHardwareCommand>
{
    public UpdateHardwareCommandValidator()
    {
        RuleFor(model => model.Price)
            .GreaterThan(0)
            .When(model => model.Price != null);
    }
}
public class UpdateHardwareCommand : IRequest<StatusResponse>
{
    [JsonIgnore] 
    public int LockerId { get; set; }
    
    [JsonIgnore] 
    public int HardwareId { get; set; }
    
    public string? Name { get; set; }
    
    public string? Code { get; set; }
    
    public string? Brand { get; set; }
    
    public double? Price { get; set; }
    
    public string? Description { get; set; }
}