using LockerService.Application.Features.Auth.Models;
using LockerService.Application.Features.Tokens.Models;

namespace LockerService.Application.Features.Auth.Commands;

public class RegisterDeviceTokenCommandValidator : AbstractValidator<RegisterDeviceTokenCommand>
{
    public RegisterDeviceTokenCommandValidator()
    {
        RuleFor(model => model.DeviceToken)
            .NotEmpty();

        RuleFor(model => model.DeviceType)
            .NotNull();
    }
}

public class RegisterDeviceTokenCommand : IRequest<TokenResponse>
{
    [TrimString(true)]
    public string DeviceToken { get; set; } = default!;
    
    public DeviceType DeviceType { get; set; } 
        
}