using LockerService.Application.Features.Auth.Models;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Auth.Commands;

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

public class CustomerLoginRequest : IRequest<AccessTokenResponse>
{
    [NormalizePhone]
    public string PhoneNumber { get; set; } = default!;
    
    [TrimString(true)]
    public string Otp { get; set; } = default!;
}