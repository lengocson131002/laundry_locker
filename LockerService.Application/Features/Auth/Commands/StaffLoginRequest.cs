using LockerService.Application.Features.Auth.Models;

namespace LockerService.Application.Features.Auth.Commands;

public class StaffLoginRequestValidator : AbstractValidator<StaffLoginRequest>
{
    public StaffLoginRequestValidator()
    {
        RuleFor(model => model.Username)
            .NotEmpty();
        
        RuleFor(model => model.Password)
            .NotEmpty();
    }
}

public class StaffLoginRequest : IRequest<TokenResponse>
{
    public string Username { get; set; } = default!;

    public string Password { get; set; } = default!;
}