using LockerService.Application.Common.Utils;

namespace LockerService.Application.Lockers.Commands;

public class ConnectLockerCommandValidator : AbstractValidator<ConnectLockerCommand> {
    
    public ConnectLockerCommandValidator()
    {
        RuleFor(model => model.Code)
            .NotEmpty();
        
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