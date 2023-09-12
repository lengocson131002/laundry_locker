namespace LockerService.Application.Auth.Commands;

public class UpdateStaffProfileCommandValidator : AbstractValidator<UpdateStaffProfileCommand>
{
    public UpdateStaffProfileCommandValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.IsValidPhoneNumber())
            .WithMessage("Invalid Phone Number");

        RuleFor(model => model.Avatar)
            .Must(image => string.IsNullOrEmpty(image) || image.IsValidUrl())
            .WithMessage("Invalid image url");
    }
}

public class UpdateStaffProfileCommand : IRequest<AccountResponse>
{
    [TrimString(true)]
    public string? FullName { get; set; }
  
    [TrimString]
    public string? Avatar { get; set; }
   
    [TrimString(true)]
    public string? Username { get; set; }
   
    [NormalizePhone(true)]
    public string? PhoneNumber { get; set; }
    
    [TrimString]
    public string? Description { get; set; }
}