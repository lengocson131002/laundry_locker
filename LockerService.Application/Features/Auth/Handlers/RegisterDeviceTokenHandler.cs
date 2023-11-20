using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Auth.Commands;
using LockerService.Application.Features.Tokens.Models;

namespace LockerService.Application.Features.Auth.Handlers;

public class RegisterDeviceTokenHandler : IRequestHandler<RegisterDeviceTokenCommand, TokenResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentAccountService _currentAccountService;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterDeviceTokenHandler> _logger;

    public RegisterDeviceTokenHandler(IUnitOfWork unitOfWork, ICurrentAccountService currentAccountService, IMapper mapper, ILogger<RegisterDeviceTokenHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentAccountService = currentAccountService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TokenResponse> Handle(RegisterDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        var existedTokens = await _unitOfWork.TokenRepository
            .Get(to => to.Value == request.DeviceToken 
                       && to.Type == TokenType.DeviceToken)
            .ToListAsync(cancellationToken);

        if (existedTokens.Any())
        {
            // delete existed device tokens
            await _unitOfWork.TokenRepository.DeleteRange(existedTokens);
        }
        
        var currentAccount = await _currentAccountService.GetRequiredCurrentAccount();
        var token = new Token()
        {
            AccountId = currentAccount.Id,
            Value = request.DeviceToken,
            Type = TokenType.DeviceToken,
            DeviceType = request.DeviceType
        };

        await _unitOfWork.TokenRepository.AddAsync(token);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TokenResponse>(token);
    }
}