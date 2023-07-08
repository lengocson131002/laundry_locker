namespace LockerService.Application.Staffs.Commands;

public class DeleteStaffCommandValidator : AbstractValidator<DeleteStaffCommand>
{
    public DeleteStaffCommandValidator()
    {
    }
}

public class DeleteStaffCommand : IRequest<StatusResponse>
{
    [JsonIgnore] public int Id { get; set; } = default!;
    [JsonIgnore] public int StoreId { get; set; } = default!;
}