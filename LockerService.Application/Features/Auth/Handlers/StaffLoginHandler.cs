using LockerService.Application.Common.Constants;
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Security;
using LockerService.Application.Features.Auth.Commands;
using LockerService.Application.Features.Auth.Models;

namespace LockerService.Application.Features.Auth.Handlers;

public class StaffLoginHandler : IRequestHandler<StaffLoginRequest, AccessTokenResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ITokenService _tokenService;

    public StaffLoginHandler(IUnitOfWork unitOfWork, 
        IJwtService jwtService, 
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _tokenService = tokenService;
    }

    public async Task<AccessTokenResponse> Handle(StaffLoginRequest request, CancellationToken cancellationToken)
    {
        var staff = await _unitOfWork.AccountRepository
            .Get(acc => Equals(acc.Username, request.Username))
            .FirstOrDefaultAsync(cancellationToken);
        
        
        if (staff == null || !BCryptUtils.Verify(request.Password, staff.Password))
        {
            throw new ApiException(ResponseCode.AuthErrorInvalidUsernameOrPassword);
        }

        // Check first login, request to update password
        if (Equals(staff.Status, AccountStatus.Verifying))
        {
            var updatePasswordToken = new Token()
            {
                Account = staff,
                Type = TokenType.ResetPassword,
                Value = Guid.NewGuid().ToString(),
                ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(TokenConstants.UpdatePasswordTokenExpireTimeInMinutes)
            };

            await _unitOfWork.TokenRepository.AddAsync(updatePasswordToken);
            await _unitOfWork.SaveChangesAsync();

            await _tokenService.SetInvalidateTokenJob(updatePasswordToken);
            
            throw new ApiException(ResponseCode.AuthErrorUpdatePasswordRequest, updatePasswordToken.Value);
        }
        
        if (!staff.IsActive)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountInactive);
        }

        var token = _jwtService.GenerateJwtToken(staff);
        var refreshToken = _jwtService.GenerateJwtRefreshToken(staff);

        return new AccessTokenResponse(token, refreshToken);
    }
}