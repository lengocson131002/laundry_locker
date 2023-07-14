namespace LockerService.Application.Staffs.Commands;

public class AssignStaffCommandValidator : AbstractValidator<AssignStaffCommand>
{
    public AssignStaffCommandValidator()
    {
        RuleFor(model => model.LockerId)
            .NotNull();
    }
}

public class AssignStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] public int Id { get; set; } = default!;
    [JsonIgnore] public int StoreId { get; set; } = default!;
    public int LockerId { get; set; } = default!;
}