using LockerService.Application.Staffs.Models;

namespace LockerService.Application.Staffs.Commands;

public class AddStaffCommandValidator : AbstractValidator<AddStaffCommand>
{
    public AddStaffCommandValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .NotEmpty();
    }
}

public class AddStaffCommand : IRequest<StaffResponse>
{
    public string PhoneNumber { get; set; } = default!;
    
    [JsonIgnore] 
    public long StoreId { get; set; } = default!;
}