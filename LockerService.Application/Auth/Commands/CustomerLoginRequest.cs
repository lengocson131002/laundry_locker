namespace LockerService.Application.Auth.Commands;

public class CustomerLoginRequestValidator : AbstractValidator<CustomerLoginRequest>
{
    public CustomerLoginRequestValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .NotEmpty();
        RuleFor(model => model.Otp)
            .NotEmpty();
    }
}

public class CustomerLoginRequest : IRequest<TokenResponse>
{
    [TrimString(true)]
    public string PhoneNumber { get; set; } = default!;
    
    [TrimString(true)]
    public string Otp { get; set; } = default!;
}