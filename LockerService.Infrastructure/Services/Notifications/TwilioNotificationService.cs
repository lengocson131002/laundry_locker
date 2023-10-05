using LockerService.Infrastructure.Settings;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace LockerService.Infrastructure.Services.Notifications;

public class TwilioNotificationService : ISmsNotificationService
{
    private readonly ILogger<TwilioNotificationService> _logger;
    private readonly TwilioSettings _twilioSettings;

    public TwilioNotificationService(
        ILogger<TwilioNotificationService> logger, 
        TwilioSettings twilioSettings)
    {
        _logger = logger;
        _twilioSettings = twilioSettings;
    }
    

    public async Task SendSmsAsync(string phoneNumber, string content)
    {
        TwilioClient.Init(_twilioSettings.AccountSID, _twilioSettings.AuthToken);

        await MessageResource.CreateAsync(
            body: content,
            from: new PhoneNumber(_twilioSettings.PhoneNumber),
            to: new PhoneNumber(phoneNumber)
        );
        _logger.LogInformation("Send SMS to {0}. Content: {1}", phoneNumber, content);
    }

    public Task NotifyAsync(Notification notification)
    {
        _logger.LogInformation("Handle SMS notification: {0}", JsonSerializer.Serialize(notification));
        return Task.CompletedTask;
    }
}