using LockerService.Application.Features.Auth.Models;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Auth.Commands;

public class CustomerVerifyRequestValidator : AbstractValidator<CustomerVerifyRequest>
{
    public CustomerVerifyRequestValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Must(x => x.IsValidPhoneNumber())
            .NotEmpty();
    }
}

public class CustomerVerifyRequest : IRequest<OtpResponse>
{
    [NormalizePhone]
    public string PhoneNumber { get; set; } = default!;
}