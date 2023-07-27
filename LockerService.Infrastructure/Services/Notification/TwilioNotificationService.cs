using LockerService.Application.Common.Services.Notification;
using LockerService.Application.Common.Services.Notification.Data;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace LockerService.Infrastructure.Services.Notification;

public class TwilioNotificationService : ISmsNotificationService
{
    private readonly ILogger<TwilioNotificationService> _logger;
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromPhoneNumber;

    public TwilioNotificationService(
        ILogger<TwilioNotificationService> logger, 
        IConfiguration configuration)
    {
        _logger = logger;
        _accountSid = configuration["Twilio:AccountSID"] ?? throw new ArgumentNullException(nameof(_accountSid));
        _authToken = configuration["Twilio:AuthToken"] ?? throw new ArgumentNullException(nameof(_authToken));
        _fromPhoneNumber = configuration["Twilio:PhoneNumber"] ?? throw new ArgumentNullException(nameof(_fromPhoneNumber));
    }

    public async Task SendAsync(SmsNotificationData data)
    {
        TwilioClient.Init(_accountSid, _authToken);

        await MessageResource.CreateAsync(
            body: data.Message,
            from: new PhoneNumber(_fromPhoneNumber),
            to: new PhoneNumber(data.Phone)
        );
        
        _logger.LogInformation("Send SMS to {0}. Content: {1}", data.Phone, data.Message);
    }
}