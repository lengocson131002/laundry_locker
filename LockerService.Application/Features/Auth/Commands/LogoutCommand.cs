namespace LockerService.Application.Features.Auth.Commands;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand> 
{
    public LogoutCommandValidator()
    {
        RuleFor(model => model.DeviceToken)
            .NotEmpty()
            .When(model => model.DeviceToken != null);
    }
}
public class LogoutCommand : IRequest<StatusResponse>
{
    [TrimString]
    public string? DeviceToken { get; set; }   
}