namespace LockerService.Application.Auth.Commands;

public class UpdateAdminProfileCommandValidator : AbstractValidator<UpdateAdminProfileCommand>
{
    public UpdateAdminProfileCommandValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.IsValidPhoneNumber())
            .WithMessage("Invalid Phone Number");
    }
}

public class UpdateAdminProfileCommand : IRequest<AccountResponse>
{
    [TrimString(true)]
    public string? FullName { get; set; }
  
    [TrimString(true)]
    public string? Avatar { get; set; }
   
    [TrimString(true)]
    public string? Username { get; set; }
   
    [TrimString(true)]
    public string? PhoneNumber { get; set; }
    
    [TrimString(true)]
    public string? Description { get; set; }
}