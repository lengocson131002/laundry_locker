using LockerService.Application.Common.Constants;

namespace LockerService.Application.Auth.Handlers;

public class StaffLoginHandler : IRequestHandler<StaffLoginRequest, TokenResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<StaffLoginHandler> _logger;
    private readonly ITokenService _tokenService;

    public StaffLoginHandler(IUnitOfWork unitOfWork, 
        ILogger<StaffLoginHandler> logger, 
        IJwtService jwtService, 
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
        _tokenService = tokenService;
    }

    public async Task<TokenResponse> Handle(StaffLoginRequest request, CancellationToken cancellationToken)
    {
        var staffQuery = await _unitOfWork.AccountRepository.GetAsync(
            predicate: account => Equals(account.PhoneNumber, request.PhoneNumber)
                                  && account.Password != null
                                  && account.Password.Equals(request.Password)
                                  && Equals(account.Role, Role.Staff)
                                  && !Equals(account.Status, AccountStatus.Inactive)
        );

        var staff = staffQuery.FirstOrDefault();
        if (staff is null)
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

        var token = _jwtService.GenerateJwtToken(staff);
        var refreshToken = _jwtService.GenerateJwtRefreshToken(staff);

        return new TokenResponse(token, refreshToken);
    }
}