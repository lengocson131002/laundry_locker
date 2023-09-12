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

    private readonly IMapper _mapper;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FirebaseNotificationService(
        ILogger<FirebaseNotificationService> logger, 
        FcmSettings fcmSettings, 
        IMapper mapper, 
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _fcmSettings = fcmSettings;
        _mapper = mapper;
        _serviceScopeFactory = serviceScopeFactory;

        
    }

    public async Task NotifyAsync(Notification notification)
    {
        // get device ids
        using var scope = _serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var deviceIds = await unitOfWork.TokenRepository
            .Get(token => Equals(TokenType.DeviceId, token.Type) && Equals(TokenStatus.Valid, token.Status))
            .ToListAsync();

        var firebaseNotificationData = _mapper.Map<NotificationModel>(notification);
        var settings = new FirebaseSettings(_fcmSettings.ProjectId, _fcmSettings.PrivateKey, _fcmSettings.ClientEmail, _fcmSettings.TokenUri);
        var fcmSender = new FirebaseSender(settings, new HttpClient());
        try
        {
            foreach (var token in deviceIds)
            {
                var firebaseNotification = new FirebaseNotification()
                {
                    Token = token.Value,
                    Data = JsonSerializer.Serialize(firebaseNotificationData)
                };
                await fcmSender.SendAsync(firebaseNotification);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError($"[MOBILE NOTIFICATION] Error when push notification: {exception.Message}");
        }
            
        _logger.LogInformation("[[MOBILE NOTIFICATION]] Handle firebase notification: {0}", notification.Id);
    }
}