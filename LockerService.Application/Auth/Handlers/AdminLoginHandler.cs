namespace LockerService.Application.Auth.Handlers;

public class AdminLoginHandler : IRequestHandler<AdminLoginRequest, TokenResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AdminLoginHandler> _logger;

    public AdminLoginHandler(IUnitOfWork unitOfWork, ILogger<AdminLoginHandler> logger, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<TokenResponse> Handle(AdminLoginRequest request, CancellationToken cancellationToken)
    {
        var userQuery = await _unitOfWork.AccountRepository.GetAsync(
            predicate: account => account.Username.ToLower().Equals(request.Username.ToLower())
                                  && account.Password != null
                                  && account.Password.Equals(request.Password)
        );

        var user = userQuery.FirstOrDefault();
        if (user is null || !Equals(user.Role, Role.Admin))
        {
            throw new ApiException(ResponseCode.AuthErrorInvalidEmailOrPassword);
        }

        var token = _jwtService.GenerateJwtToken(user);
        var refreshToken = _jwtService.GenerateJwtRefreshToken(user);

        return new TokenResponse(token, refreshToken);
    }
}