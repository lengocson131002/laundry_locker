namespace LockerService.Application.Auth.Commands;

public class StaffLoginRequestValidator : AbstractValidator<StaffLoginRequest>
{
    public StaffLoginRequestValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Must(x => x.IsValidPhoneNumber())
            .NotEmpty()
            .WithMessage("Invalid phone number");
        
        RuleFor(model => model.Password)
            .NotEmpty();
    }
}

public class StaffLoginRequest : IRequest<TokenResponse>
{
    [NormalizePhone]
    public string PhoneNumber { get; set; } = default!;

    public string Password { get; set; } = default!;
}