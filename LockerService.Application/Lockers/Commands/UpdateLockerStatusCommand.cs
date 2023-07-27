namespace LockerService.Application.Lockers.Commands;

public class UpdateLockerStatusCommandValidator : AbstractValidator<UpdateLockerStatusCommand>
{
    public UpdateLockerStatusCommandValidator()
    {
        RuleFor(model => model.Status)
            .IsInEnum()
            .NotNull()
            .Must(status => !Equals(LockerStatus.Initialized, status) && !Equals(LockerStatus.Disconnected, status))
            .WithMessage("Invalid request locker status");
    }
}

public class UpdateLockerStatusCommand : IRequest
{
    [JsonIgnore] 
    public long LockerId { get; set; }
    
    public LockerStatus Status { get; set; }
}