namespace LockerService.Application.Services.Commands;

public class UpdateServiceStatusCommandValidator : AbstractValidator<UpdateServiceStatusCommand>
{
    public UpdateServiceStatusCommandValidator()
    {
        RuleFor(model => model.Status)
            .NotNull();
    }
}

public class UpdateServiceStatusCommand : IRequest
{
    [JsonIgnore]
    public long ServiceId { get; set; }
    
    public ServiceStatus Status { get; set; }
}
