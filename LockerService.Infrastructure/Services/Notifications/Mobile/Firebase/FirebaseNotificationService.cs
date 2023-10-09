using AutoMapper;
using CorePush.Firebase;
using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace LockerService.Infrastructure.Services.Notifications.Mobile.Firebase;

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
        var notiAdapter = scope.ServiceProvider.GetRequiredService<INotificationAdapter>();
        
        var deviceIds = await unitOfWork.TokenRepository
            .Get(token => Equals(TokenType.DeviceToken, token.Type) && Equals(TokenStatus.Valid, token.Status))
            .ToListAsync();

        var settings = new FirebaseSettings(_fcmSettings.ProjectId, _fcmSettings.PrivateKey, _fcmSettings.ClientEmail, _fcmSettings.TokenUri);
        var fcmSender = new FirebaseSender(settings, new HttpClient());
        
        _logger.LogInformation("Firebase key: {0}", _fcmSettings.PrivateKey);
        foreach (var token in deviceIds)
        {
            try
            {
                var firebaseNotification = await notiAdapter.ToFirebaseNotification(notification, token.Value);
                await fcmSender.SendAsync(firebaseNotification);
            }
            catch (Exception exception)
            {
                _logger.LogError($"[MOBILE NOTIFICATION] Error when push notification: {exception.Message}");
            }
        }
        _logger.LogInformation("[[MOBILE NOTIFICATION]] Handle firebase notification: {0}", notification.Id);
    }
}