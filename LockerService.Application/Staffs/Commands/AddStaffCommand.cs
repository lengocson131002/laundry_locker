namespace LockerService.Application.Staffs.Commands;

public class AddStaffCommandValidator : AbstractValidator<AddStaffCommand>
{
    public AddStaffCommandValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Must(phoneNumber => phoneNumber.IsValidPhoneNumber())
            .WithMessage("Invalid Phone Number");

        RuleFor(model => model.FullName)
            .NotEmpty();

        RuleFor(model => model.Password)
            .Must(phoneNumber => phoneNumber.IsValidPassword())
            .WithMessage("Invalid Password");

        RuleFor(model => model.StoreId)
            .NotNull();
    }
}

public class AddStaffCommand : IRequest<StaffDetailResponse>
{
    public string FullName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

    public string? Avatar { get; set; }

    public string Password { get; set; } = default!;

    public string? Description { get; set; }

    public long StoreId { get; set; } = default!;
}