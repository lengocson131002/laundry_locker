namespace LockerService.Application.Auth.Commands;

public class StaffLoginRequestValidator : AbstractValidator<StaffLoginRequest>
{
    public StaffLoginRequestValidator()
    {
        RuleFor(model => model.PhongNumber)
            .NotEmpty();
        RuleFor(model => model.Password)
            .NotEmpty();
    }
}

public class StaffLoginRequest : IRequest<TokenResponse>
{
    [TrimString(true)]
    public string PhongNumber { get; set; } = default!;

    public string Password { get; set; } = default!;
}