namespace LockerService.Application.Auth.Commands;

public class CustomerLoginRequestValidator : AbstractValidator<CustomerLoginRequest>
{
    public CustomerLoginRequestValidator()
    {
        RuleFor(model => model.Username)
            .NotEmpty();
        RuleFor(model => model.Otp)
            .NotEmpty();
    }
}

public class CustomerLoginRequest : IRequest<TokenResponse>
{
    public string Username { get; set; } = default!;
    public string Otp { get; set; } = default!;
}