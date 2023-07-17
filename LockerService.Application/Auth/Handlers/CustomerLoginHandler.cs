namespace LockerService.Application.Auth.Handlers;

public class CustomerLoginHandler : IRequestHandler<CustomerLoginRequest, TokenResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<CustomerLoginHandler> _logger;

    public CustomerLoginHandler(IUnitOfWork unitOfWork, ILogger<CustomerLoginHandler> logger, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<TokenResponse> Handle(CustomerLoginRequest request, CancellationToken cancellationToken)
    {
        var accountQuery = await _unitOfWork.AccountRepository.GetAsync(
            predicate: account => Equals(account.Username, request.Username)
                                  && Equals(account.Role, Role.Customer)
        );

        var account = accountQuery.FirstOrDefault();
        if (account is null)
        {
            throw new ApiException(ResponseCode.AuthErrorInvalidUsernameOrOtp);
        }

        var otpQuery = await _unitOfWork.TokenRepository.GetAsync(
            predicate: token => token.AccountId == account.Id
                                && Equals(token.Value, request.Otp)
                                && Equals(token.Type, TokenType.Otp)
                                && Equals(token.Status, TokenStatus.Valid)
                                && (token.ExpiredAt == null ||
                                    token.ExpiredAt > DateTimeOffset.UtcNow)
        );

        var otp = otpQuery.FirstOrDefault();
        if (otp is null)
        {
            throw new ApiException(ResponseCode.AuthErrorInvalidUsernameOrOtp);
        }

        otp.Status = TokenStatus.Invalid;
        await _unitOfWork.TokenRepository.UpdateAsync(otp);

        await _unitOfWork.SaveChangesAsync();

        var token = _jwtService.GenerateJwtToken(account);
        var refreshToken = _jwtService.GenerateJwtRefreshToken(account);

        return new TokenResponse(token, refreshToken);
    }
}