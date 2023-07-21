namespace LockerService.Application.Auth.Commands;

public class UpdateAdminProfileCommandValidator : AbstractValidator<UpdateAdminProfileCommand>
{
    public UpdateAdminProfileCommandValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Must(phoneNumber => phoneNumber.IsValidPhoneNumber())
            .WithMessage("Invalid Phone Number")
            .When(model => !string.IsNullOrEmpty(model.PhoneNumber));
    }
}

public class UpdateAdminProfileCommand : IRequest<AccountResponse>
{
    public string? FullName { get; set; }

    public string? Avatar { get; set; }

    public string? Username { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Description { get; set; }
}