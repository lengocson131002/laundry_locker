namespace LockerService.Application.Staffs.Commands;

public class AddStaffCommandValidator : AbstractValidator<AddStaffCommand>
{
    public AddStaffCommandValidator()
    {
        RuleFor(model => model.Username)
            .NotEmpty();
        
        RuleFor(model => model.PhoneNumber)
            .NotNull()
            .Must(phoneNumber => phoneNumber.IsValidPhoneNumber())
            .WithMessage("Invalid Phone Number");

        RuleFor(model => model.FullName)
            .NotEmpty();

        RuleFor(model => model.Password)
            .NotEmpty()
            .Must(password => password.IsValidPassword())
            .WithMessage("Invalid password format");

        RuleFor(model => model.StoreId)
            .GreaterThan(0);
        
        RuleFor(model => model.Avatar)
            .Must(image => image == null || image.IsValidUrl())
            .WithMessage("Invalid image url");

        RuleFor(model => model.Role)
            .NotNull()
            .Must(role => Equals(Role.Manager, role) || Equals(Role.Shipper, role) || Equals(Role.LaundryAttendant, role));
    }
}

public class AddStaffCommand : IRequest<StaffDetailResponse>
{
    [TrimString(true)] 
    public string Username { get; set; } = default!;
    
    [TrimString(true)]
    public string FullName { get; set; } = default!;
    
    [NormalizePhone]
    public string PhoneNumber { get; set; } = default!;
   
    [TrimString(true)]
    public string? Avatar { get; set; }

    [TrimString(true)]
    public string Password { get; set; } = default!;

    [TrimString(true)]
    public string? Description { get; set; }

    public long StoreId { get; set; }
    
    public Role Role { get; set; }
}