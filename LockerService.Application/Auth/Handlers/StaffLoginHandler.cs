namespace LockerService.Application.Auth.Handlers;

public class StaffLoginHandler : IRequestHandler<StaffLoginRequest, TokenResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<StaffLoginHandler> _logger;

    public StaffLoginHandler(IUnitOfWork unitOfWork, ILogger<StaffLoginHandler> logger, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<TokenResponse> Handle(StaffLoginRequest request, CancellationToken cancellationToken)
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