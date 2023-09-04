namespace LockerService.Application.Auth.Commands;

public class CustomerLoginRequestValidator : AbstractValidator<CustomerLoginRequest>
{
    public CustomerLoginRequestValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Must(x => x.IsValidPhoneNumber())
            .NotEmpty();
        RuleFor(model => model.Otp)
            .NotEmpty();
    }
}

public class CustomerLoginRequest : IRequest<TokenResponse>
{
    [NormalizePhone]
    public string PhoneNumber { get; set; } = default!;
    
    [TrimString(true)]
    public string Otp { get; set; } = default!;
}