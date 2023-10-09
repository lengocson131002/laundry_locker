using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Auth.Commands;
using LockerService.Application.Features.Auth.Models;

namespace LockerService.Application.Features.Auth.Handlers;

public class AdminLoginHandler : IRequestHandler<AdminLoginRequest, AccessTokenResponse>
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

    public async Task<AccessTokenResponse> Handle(AdminLoginRequest request, CancellationToken cancellationToken)
    {
        var userQuery = await _unitOfWork.AccountRepository.GetAsync(
            predicate: account => Equals(account.Username, request.Username)
                                  && account.Password != null
                                  && Equals(account.Password, request.Password)
                                  && Equals(account.Role, Role.Admin)
        );

        var user = userQuery.FirstOrDefault();
        if (user is null)
        {
            throw new ApiException(ResponseCode.AuthErrorInvalidUsernameOrPassword);
        }

        var token = _jwtService.GenerateJwtToken(user);
        var refreshToken = _jwtService.GenerateJwtRefreshToken(user);

        return new AccessTokenResponse(token, refreshToken);
    }
}