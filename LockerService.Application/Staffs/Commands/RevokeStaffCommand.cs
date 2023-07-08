namespace LockerService.Application.Staffs.Commands;

public class RevokeStaffCommandValidator : AbstractValidator<RevokeStaffCommand>
{
    public RevokeStaffCommandValidator()
    {
        RuleFor(model => model.LockerId)
            .NotNull();
    }
}

public class RevokeStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] public int Id { get; set; } = default!;
    [JsonIgnore] public int StoreId { get; set; } = default!;
    public int LockerId { get; set; } = default!;
}