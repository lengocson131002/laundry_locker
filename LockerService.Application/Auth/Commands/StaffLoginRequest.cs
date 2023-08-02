namespace LockerService.Application.Auth.Commands;

public class StaffLoginRequestValidator : AbstractValidator<StaffLoginRequest>
{
    public StaffLoginRequestValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Must(x => x.IsValidPhoneNumber())
            .NotEmpty();
        RuleFor(model => model.Password)
            .NotEmpty();
    }
}

public class StaffLoginRequest : IRequest<TokenResponse>
{
    [TrimString(true)]
    public string PhoneNumber { get; set; } = default!;

    public string Password { get; set; } = default!;
}