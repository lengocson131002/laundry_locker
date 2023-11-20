using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Auth.Commands;

namespace LockerService.Application.Features.Auth.Handlers;

public class LogoutHandler : IRequestHandler<LogoutCommand, StatusResponse>
{
    private readonly ICurrentAccountService _currentAccountService;

    private readonly IUnitOfWork _unitOfWork;


    public LogoutHandler(ICurrentAccountService currentAccountService, IUnitOfWork unitOfWork)
    {
        _currentAccountService = currentAccountService;
        _unitOfWork = unitOfWork;
    }

    public async Task<StatusResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var existedTokens = await _unitOfWork.TokenRepository
            .Get(to => to.Value == request.DeviceToken 
                       && to.Type == TokenType.DeviceToken)
            .ToListAsync(cancellationToken);

        if (existedTokens.Any())
        {
            // delete existed device tokens
            await _unitOfWork.TokenRepository.DeleteRange(existedTokens);
            await _unitOfWork.SaveChangesAsync();
        }

        return new StatusResponse();
    }
}