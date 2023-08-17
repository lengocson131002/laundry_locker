using AutoMapper;
using CorePush.Firebase;
using LockerService.Application.Common.Services.Notifications;
using LockerService.Application.Notifications.Models;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace LockerService.Infrastructure.Services.Notifications;

public class FirebaseNotificationService : IMobileNotificationService
{
    private readonly ILogger<FirebaseNotificationService> _logger;
    
    private readonly FcmSettings _fcmSettings;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public FirebaseNotificationService(
        IServiceProvider serviceProvider,
        ILogger<FirebaseNotificationService> logger, 
        FcmSettings fcmSettings, 
        IMapper mapper)
    {
        _logger = logger;
        _fcmSettings = fcmSettings;
        _mapper = mapper;

        using var scope = serviceProvider.CreateScope();
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    public async Task NotifyAsync(Notification notification)
    {
        // get device ids
        var deviceIds = await _unitOfWork.TokenRepository
            .Get(token => Equals(TokenType.DeviceId, token.Type) && Equals(TokenStatus.Valid, token.Status))
            .ToListAsync();

        try
        {
            foreach (var token in deviceIds)
            {
                var settings = new FirebaseSettings(_fcmSettings.ProjectId, _fcmSettings.PrivateKey, _fcmSettings.ClientEmail, _fcmSettings.TokenUri);
                var fcmSender = new FirebaseSender(settings, new HttpClient());
                var firebaseNotification = new FirebaseNotification()
                {
                    Token = token.Value,
                    Data = JsonSerializer.Serialize(_mapper.Map<NotificationModel>(notification))
                };
                await fcmSender.SendAsync(firebaseNotification);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError($"[MOBILE NOTIFICATION] Error when push notification: {exception.Message}");
        }
            
        _logger.LogInformation("[[MOBILE NOTIFICATION]] Handle firebase notification: {0}", JsonSerializer.Serialize(notification));
    }
}