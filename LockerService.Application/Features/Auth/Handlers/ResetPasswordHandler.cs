using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Security;
using LockerService.Application.Features.Auth.Commands;

namespace LockerService.Application.Features.Auth.Handlers;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StatusResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // Validate username
        var account = await _unitOfWork
            .AccountRepository.Get((acc => Equals(acc.Username, request.Username)))
            .FirstOrDefaultAsync(cancellationToken);

        if (account == null)
        {
            throw new ApiException(ResponseCode.AuthErrorAccountNotFound);
        }
        
        // validate token
        var token = await _unitOfWork.TokenRepository
            .Get(token => Equals(token.Type, TokenType.ResetPassword) 
                          && Equals(token.Value, request.Token)
                          && Equals(token.AccountId, account.Id)
                          && !token.IsExpired)
            .Include(token => token.Account)
            .FirstOrDefaultAsync(cancellationToken);

        if (token == null)
        {
            throw new ApiException(ResponseCode.TokenErrorInvalidOrExpiredToken);
        }
        
        // update password
        account.Status = AccountStatus.Active;
        account.Password = BCryptUtils.Hash(request.Password);
        await _unitOfWork.AccountRepository.UpdateAsync(account);
        
        // Invalidate token
        token.Status = TokenStatus.Invalid;
        await _unitOfWork.TokenRepository.UpdateAsync(token);

        // Save changes
        await _unitOfWork.SaveChangesAsync();

        return new StatusResponse(true);
    }
}