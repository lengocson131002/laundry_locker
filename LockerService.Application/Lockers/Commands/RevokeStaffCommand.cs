namespace LockerService.Application.Staffs.Commands;

public class RevokeStaffCommandValidator : AbstractValidator<RevokeStaffCommand>
{
    public RevokeStaffCommandValidator()
    {
        RuleFor(model => model.LockerId)
            .NotNull();
        
        RuleFor(model => model.StaffId)
            .NotNull();
    }
}

public class RevokeStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] public long LockerId { get; set; } = default!;

    public long StaffId { get; set; } = default!;
}