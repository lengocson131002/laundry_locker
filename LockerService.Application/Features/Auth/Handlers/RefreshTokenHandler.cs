using System.Security.Claims;
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Auth.Commands;
using LockerService.Application.Features.Auth.Models;

namespace LockerService.Application.Features.Auth.Handlers;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenRequest, TokenResponse>
{
    private readonly ICurrentPrincipalService _currentUserService;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenHandler(
        ICurrentPrincipalService currentUserService,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var principal = _currentUserService.GetCurrentPrincipalFromToken(request.RefreshToken);
        var accountId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (accountId == null)
        {
            throw new ApiException(ResponseCode.AuthErrorInvalidRefreshToken);
        }

        var accountQuery = await _unitOfWork.AccountRepository.GetAsync(
            account => Equals(accountId, account.Id.ToString())
        );

        var account = accountQuery.FirstOrDefault();
        if (account == null)
        {
            throw new ApiException(ResponseCode.AuthErrorInvalidRefreshToken);
        }

        var newAccessToken = _jwtService.GenerateJwtToken(account);
        var newRefreshToken = _jwtService.GenerateJwtRefreshToken(account);

        return new TokenResponse(newAccessToken, newRefreshToken);
    }
}