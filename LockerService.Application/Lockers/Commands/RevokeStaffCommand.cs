namespace LockerService.Application.Staffs.Commands;

public class RevokeStaffCommandValidator : AbstractValidator<RevokeStaffCommand>
{
    public RevokeStaffCommandValidator()
    {
        RuleFor(model => model.StaffId)
            .NotNull();
    }
}

public class RevokeStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] public long LockerId { get; set; }

    public long StaffId { get; set; } = default!;
}