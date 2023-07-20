namespace LockerService.Application.Lockers.Commands;

public class AssignStaffCommandValidator : AbstractValidator<AssignStaffCommand>
{
    public AssignStaffCommandValidator()
    {
        RuleFor(model => model.StaffId)
            .NotNull();
    }
}

public class AssignStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] public long LockerId { get; set; } = default!;
    public long StaffId { get; set; } = default!;
}