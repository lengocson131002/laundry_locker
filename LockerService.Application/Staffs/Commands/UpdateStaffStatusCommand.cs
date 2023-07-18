namespace LockerService.Application.Staffs.Commands;

public class UpdateStaffStatusCommandValidator : AbstractValidator<UpdateStaffStatusCommand>
{
    public UpdateStaffStatusCommandValidator()
    {
        RuleFor(model => model.Status)
            .NotNull();
    }
}

public class UpdateStaffStatusCommand : IRequest<StaffDetailResponse>
{
    [JsonIgnore] public long Id { get; set; } = default!;

    public AccountStatus Status { get; set; }
}