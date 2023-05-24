using LockerService.Application.Common.Extensions;

namespace LockerService.Application.Lockers.Commands;

public class ConnectLockerCommandValidator : AbstractValidator<ConnectLockerCommand> {
    
    public ConnectLockerCommandValidator()
    {
        RuleFor(model => model.MacAddress)
            .Must(model => model.IsValidMacAddress())
            .WithMessage("Invalid MAC address. Right format XX:XX:XX:XX:XX:XX");
    }
}

public class ConnectLockerCommand : IRequest<LockerResponse>
{
    public string MacAddress { get; set; } = default!;
}