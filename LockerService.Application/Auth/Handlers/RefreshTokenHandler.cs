using LockerService.Application.Auth.Commands;
using LockerService.Application.Auth.Models;

namespace LockerService.Application.Auth.Handlers;

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
        var accId = _currentUserService.CurrentSubjectId;
        if (accId == null)
        {
            throw new ApiException(ResponseCode.AuthErrorInvalidRefreshToken);
        }
        
        var accountQuery = await _unitOfWork.AccountRepository.GetAsync(account => account.Id == accId);

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