using LockerService.Application.Staffs.Models;

namespace LockerService.Application.Staffs.Commands;

public class AddStaffCommandValidator : AbstractValidator<AddStaffCommand>
{
    public AddStaffCommandValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .NotEmpty();

        RuleFor(model => model.FullName)
            .NotEmpty();

        RuleFor(model => model.StoreId)
            .NotNull();
    }
}

public class AddStaffCommand : IRequest<StaffDetailResponse>
{
    public string FullName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

    public string Avatar { get; set; }

    public string Description { get; set; }

    public long StoreId { get; set; }
}