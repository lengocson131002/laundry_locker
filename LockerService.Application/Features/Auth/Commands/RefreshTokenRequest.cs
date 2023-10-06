using LockerService.Application.Features.Auth.Models;

namespace LockerService.Application.Features.Auth.Commands;


public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(model => model.RefreshToken)
            .NotEmpty();
    }
}

public class RefreshTokenRequest : IRequest<TokenResponse>
{
    public string RefreshToken { get; set; } = default!;
}