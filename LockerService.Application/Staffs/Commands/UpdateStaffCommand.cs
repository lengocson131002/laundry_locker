
namespace LockerService.Application.Staffs.Commands;

public class UpdateStaffCommandValidator : AbstractValidator<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        
    }
}

public class UpdateStaffCommand : IRequest<StaffDetailResponse>
{
    [JsonIgnore] public long Id { get; set; } = default!;

    public string? FullName { get; set; }

    public string? Avatar { get; set; }

    public string? Description { get; set; }

    public long? StoreId { get; set; }
}