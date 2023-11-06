using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Lockers.Commands;
using LockerService.Shared.Utils;

namespace LockerService.Application.Features.Lockers.Handlers;

public class OpenBoxHandler : IRequestHandler<OpenBoxCommand, StatusResponse>
{
    private readonly ICacheService _cacheService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMqttBus _mqttBus;

    public OpenBoxHandler(ICacheService cacheService, IUnitOfWork unitOfWork, IMqttBus mqttBus)
    {
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _mqttBus = mqttBus;
    }

    public async Task<StatusResponse> Handle(OpenBoxCommand request, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(request.LockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }
        
        // Check box
        var box = await _unitOfWork.BoxRepository.FindBox(request.LockerId, request.BoxNumber);
        if (box == null)
        {
            throw new ApiException(ResponseCode.LockerErrorBoxNotFound);
        }

        if (!box.IsActive)
        {
            throw new ApiException(ResponseCode.LockerErrorInvalidBoxStatus);
        }
        
        // Get token from redis
        var tokenKey = RedisUtils.GenerateOpenBoxTokenKey(request.LockerId, request.Token);
        
        var token = await _cacheService.GetAsync<string>(tokenKey, cancellationToken);
        
        if (token == null || !Equals(token, request.Token))
        {
            throw new ApiException(ResponseCode.TokenErrorInvalidOrExpiredToken);
        }

        // Push MQTT Event to open box'
        await _mqttBus.PublishAsync(new MqttOpenBoxEvent()
        {
            LockerCode = locker.Code,
            BoxNumber = request.BoxNumber
        });
        
        // Remove token
        await _cacheService.RemoveAsync(tokenKey, cancellationToken);
        
        return new StatusResponse();
    }
}