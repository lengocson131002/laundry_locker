namespace LockerService.Application.Auth.Handlers;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StatusResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // validate token
        var token = await _unitOfWork.TokenRepository
            .Get(token => Equals(token.Type, TokenType.ResetPassword) 
                          && Equals(token.Value, request.Token) 
                          && !token.IsExpired)
            .Include(token => token.Account)
            .FirstOrDefaultAsync(cancellationToken);

        if (token == null)
        {
            throw new ApiException(ResponseCode.TokenErrorInvalidOrExpiredToken);
        }
        
        // update password
        var account = token.Account;
        account.Status = AccountStatus.Active;
        account.Password = request.Password;
        await _unitOfWork.AccountRepository.UpdateAsync(account);
        
        // Invalidate token
        token.Status = TokenStatus.Invalid;
        await _unitOfWork.TokenRepository.UpdateAsync(token);

        // Save changes
        await _unitOfWork.SaveChangesAsync();

        return new StatusResponse(true);
    }
}