using LockerService.Application.Features.Auth.Models;

namespace LockerService.Application.Features.Auth.Commands;

public class AdminLoginRequestValidator : AbstractValidator<AdminLoginRequest>
{
    public AdminLoginRequestValidator()
    {
        RuleFor(model => model.Username)
            .NotEmpty();
            
        RuleFor(model => model.Password)
            .NotEmpty();
    }
}

public class AdminLoginRequest : IRequest<AccessTokenResponse>
{
    [TrimString(true)]
    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;
}