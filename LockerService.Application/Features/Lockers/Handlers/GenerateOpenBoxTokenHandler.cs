using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Lockers.Commands;
using LockerService.Application.Features.Tokens.Models;
using LockerService.Shared.Utils;

namespace LockerService.Application.Features.Lockers.Handlers;

public class GenerateOpenBoxTokenHandler : IRequestHandler<GenerateOpenBoxTokenCommand, TokenResponse>
{
    private readonly ICacheService _cacheService;

    private readonly IUnitOfWork _unitOfWork;

    private const int OpenBoxTokenDuration = 5;

    public GenerateOpenBoxTokenHandler(ICacheService cacheService, IUnitOfWork unitOfWork)
    {
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TokenResponse> Handle(GenerateOpenBoxTokenCommand request, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(request.LockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        // generate token
        var token = TokenUtils.GenerateRandomToken();

        // Save token in redis
        var cacheKey = RedisUtils.GenerateOpenBoxTokenKey(locker.Id, token);
        var expireDuration = TimeSpan.FromMinutes(OpenBoxTokenDuration);

        await _cacheService.SetWithExpirationAsync(
            cacheKey,
            token,
            expireDuration,
            cancellationToken);

        return new TokenResponse()
        {
            Type = TokenType.Other,
            Value = token,
            ExpiredAt = DateTimeOffset.UtcNow.Add(expireDuration)
        };
    }
}