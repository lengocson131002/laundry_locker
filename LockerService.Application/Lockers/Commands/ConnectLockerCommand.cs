using LockerService.Application.Common.Extensions;
using LockerService.Application.Common.Utils;

namespace LockerService.Application.Lockers.Commands;

public class ConnectLockerCommandValidator : AbstractValidator<ConnectLockerCommand> {
    
    public ConnectLockerCommandValidator()
    {
        RuleFor(model => model.Code)
            .NotEmpty();

        RuleFor(model => model.MacAddress)
            .Must(mac => mac == null || mac.IsValidMacAddress())
            .WithMessage("Invalid MAC address");

        RuleFor(model => model.IpAddress)
            .Must(ip => ip == null || ip.IsValidIpAddress())
            .WithMessage("Invalid IP address");
    }
}

public class ConnectLockerCommand : IRequest<LockerResponse>
{
    [TrimString(true)]
    public string Code { get; set; } = default!;

    [TrimString(true)]
    public string? MacAddress { get; set; } = default!;

    [TrimString(true)]
    public string? IpAddress { get; set; }
    
}