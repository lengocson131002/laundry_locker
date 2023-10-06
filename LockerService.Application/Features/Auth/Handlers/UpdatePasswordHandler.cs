using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Security;
using LockerService.Application.Features.Auth.Commands;

namespace LockerService.Application.Features.Auth.Handlers;

public class UpdatePasswordHandler : IRequestHandler<UpdatePasswordCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UpdatePasswordHandler> _logger;
    private readonly ICurrentPrincipalService _currentUserService;

    public UpdatePasswordHandler(IUnitOfWork unitOfWork, ILogger<UpdatePasswordHandler> logger, IJwtService jwtService,
        ICurrentPrincipalService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
        _currentUserService = currentUserService;
    }

    public async Task<StatusResponse> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var accountId = _currentUserService.CurrentPrincipal;
        if (accountId is null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        var accountQuery = await _unitOfWork.AccountRepository.GetAsync(
            predicate: account => Equals(long.Parse(accountId), account.Id)
        );

        var account = accountQuery.FirstOrDefault();
        if (account is null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }

        if (!BCryptUtils.Verify(request.CurrentPassword, account.Password))
        {
            throw new ApiException(ResponseCode.AuthErrorCurrentPasswordIncorrect);
        }

        if (Equals(request.NewPassword, account.Password))
        {
            throw new ApiException(ResponseCode.AuthErrorNewPasswordMustBeDifferent);
        }

        account.Password = BCryptUtils.Hash(request.NewPassword);

        await _unitOfWork.AccountRepository.UpdateAsync(account);

        await _unitOfWork.SaveChangesAsync();

        return new StatusResponse(true);
    }
}