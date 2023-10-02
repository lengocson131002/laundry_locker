namespace LockerService.Application.Hardwares.Commands;

public class UpdateHardwareCommandValidator : AbstractValidator<UpdateHardwareCommand>
{
    public UpdateHardwareCommandValidator()
    {
        RuleFor(model => model.Price)
            .GreaterThan(0)
            .When(model => model.Price != null);
        
        RuleFor(model => model.Price)
            .GreaterThan(0)
            .When(model => model.Count != null);
    }
}
public class UpdateHardwareCommand : IRequest<HardwareResponse>
{
    [JsonIgnore] 
    public long LockerId { get; set; }
    
    [JsonIgnore] 
    public long HardwareId { get; set; }
    
    [TrimString(true)]
    public string? Name { get; set; }
    
    [TrimString(true)]
    public string? Code { get; set; }
    
    [TrimString(true)]
    public string? Brand { get; set; }
    
    public decimal? Price { get; set; }
    
    public long? Count { get; set; }
    
    [TrimString(true)]
    public string? Description { get; set; }
}