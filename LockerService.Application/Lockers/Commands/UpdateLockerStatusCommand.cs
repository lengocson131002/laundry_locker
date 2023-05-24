namespace LockerService.Application.Lockers.Commands;

public class UpdateLockerStatusCommandValidator : AbstractValidator<UpdateLockerStatusCommand>
{
    public UpdateLockerStatusCommandValidator()
    {
        RuleFor(model => model.Status)
            .IsInEnum()
            .NotNull();
    }
}

public class UpdateLockerStatusCommand : IRequest
{
    [JsonIgnore] 
    public int LockerId { get; set; }
    
    public LockerStatus Status { get; set; }
}