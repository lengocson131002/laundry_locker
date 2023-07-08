namespace LockerService.Application.Staffs.Commands;

public class ActivateStaffCommandValidator : AbstractValidator<ActivateStaffCommand>
{
}

public class ActivateStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] public int Id { get; set; } = default!;
    [JsonIgnore] public int StoreId { get; set; } = default!;
}