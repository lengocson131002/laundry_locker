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
            .NotEmpty();

        RuleFor(model => model.StoreId)
            .NotNull();
    }
}

public class AddStaffCommand : IRequest<StaffDetailResponse>
{
    [TrimString(true)]
    public string FullName { get; set; } = default!;
    
    [TrimString(true)]
    public string PhoneNumber { get; set; } = default!;
   
    [TrimString(true)]
    public string? Avatar { get; set; }

    public string Password { get; set; } = default!;

    [TrimString(true)]
    public string? Description { get; set; }

    public long StoreId { get; set; } = default!;
}