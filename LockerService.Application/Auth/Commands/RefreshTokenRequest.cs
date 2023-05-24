using LockerService.Application.Auth.Models;

namespace LockerService.Application.Auth.Commands;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
}

public class RefreshTokenRequest : IRequest<TokenResponse>
{
}