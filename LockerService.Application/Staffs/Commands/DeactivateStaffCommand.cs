namespace LockerService.Application.Staffs.Commands;

public class DeactivateStaffCommandValidator : AbstractValidator<DeactivateStaffCommand>
{
    public DeactivateStaffCommandValidator()
    {
    }
}

public class DeactivateStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] public int Id { get; set; } = default!;
    [JsonIgnore] public int StoreId { get; set; } = default!;
}