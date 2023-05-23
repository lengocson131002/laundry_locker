using FluentValidation;
using LockerService.Application.Auth.Models;
using MediatR;

namespace LockerService.Application.Auth.Commands;

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